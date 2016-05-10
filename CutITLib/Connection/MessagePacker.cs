using CutIT.Messages;
using CutIT.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CutIT.Connection
{
    public class MessagePacker
    {
        private ConcurrentQueue<char[]> _rxData { get; set; }
        private ConcurrentQueue<char[]> _txData { get; set; }
        private CancellationTokenSource _threadCancelSource = null;
        private CancellationTokenSource _requestCancelSource = null;
        private Thread _runThread { get; set; }

        public ConcurrentObservableCollection<GrblRequest> RequestsToDo { get; protected set; }
        public ConcurrentObservableCollection<GrblRequest> RequestsDone { get; protected set; }
        public ConcurrentObservableCollection<GrblResponse> Responses { get; protected set; }             
        public ConcurrentObservableCollection<GrblRequest> SpecialRequests { get; protected set; }
        public ConcurrentObservableCollection<GrblRequest> RequestsRejected { get; protected set; }

        public bool PauseOnReject { get; set; }
        public int RequestTimeout { get; set; }
        public bool IsPaused { get; protected set; }
        public virtual bool IsRunning { get; set; }        
        public virtual bool SaveRejectedRequests { get; set; }
        public bool AllowSpecialWhenHalted { get; set; }

        private bool isRunning = false;
        private GrblRequest _requestDoing = null;
        private GrblResponse _responseDoing = null;           
        
        public MessagePacker()
        {
            _rxData = null;
            _txData = null;
            SaveRejectedRequests = true;
            RequestsToDo = new ConcurrentObservableCollection<GrblRequest>();
            RequestsDone = new ConcurrentObservableCollection<GrblRequest>();
            SpecialRequests = new ConcurrentObservableCollection<GrblRequest>();
            Responses = new ConcurrentObservableCollection<GrblResponse>();
            RequestsRejected = new ConcurrentObservableCollection<GrblRequest>();
            _requestDoing = null;
            _responseDoing = null;
            RequestTimeout = 2000;
            PauseOnReject = true;
            AllowSpecialWhenHalted = true;
        }

        public MessagePacker(ConcurrentQueue<char[]> RxData, ConcurrentQueue<char[]> TxData):this()
        {
            _rxData = RxData;
            _txData = TxData;
        }

        public virtual bool Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                _threadCancelSource = new CancellationTokenSource();
                _runThread = new Thread(new ThreadStart(ThreadMain));
                _runThread.Start();
                return true;
            }
            return false;
        }

        public virtual bool Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _threadCancelSource.Cancel();
                while (_runThread != null) ;
                return true;
            }
            return false;
        }

        public virtual void Resume()
        {
            IsPaused = false;
        }

        public virtual void Pause()
        {
            IsPaused = true;
        }

        public virtual bool Add(GrblRequest request)
        {
            if (IsRunning)
            {
                if (request.IsValid)
                {
                    if (request.IsSpecial)
                        SpecialRequests.Add(request);
                    else
                        RequestsToDo.Add(request);
                    return true;
                }
                else if(SaveRejectedRequests)
                    RequestsRejected.Add(request);
            }
            return false;   
        }

        public virtual bool Add(string line)
        {
            GrblRequest request = new GrblRequest();
            request.SetContent(line);
            if (request.IsRequestType(GrblRequestEnum.GCode))
            {
                request = new GCommand(line,true);
            }
            return Add(request);
        }

        protected virtual int GetAndSendSpecial()
        {
            int result = 0;
            while (SpecialRequests.Count > 0)
            {
                GrblRequest specialRequest = SpecialRequests.Pop();
                if (specialRequest.IsRequestType(GrblRequestEnum.CurrentStatus))
                    RequestsToDo.Insert(0, specialRequest);
                else
                {
                    specialRequest.Stamp();
                    _txData.Enqueue(specialRequest.Content.ToCharArray());
                    RequestsDone.Add(specialRequest);
                    result++;
                }
            }
            return result;
        }

        protected virtual bool GetAndSendRequest()
        {
            bool result = false;
            if (_requestDoing == null)
            {
                RequestsToDo.TryDequeue(out _requestDoing);
                if (_requestDoing != null)
                {
                    if (_requestDoing.IsValid && !_requestDoing.IsStamped)
                    {
                        _requestDoing.Stamp();
                        _txData.Enqueue((_requestDoing.Content + "\n").ToCharArray());
                        _requestCancelSource.Dispose();
                        _requestCancelSource = new CancellationTokenSource();
                        //_requestCancelSource.CancelAfter(RequestTimeout);
                        result = true;
                    }
                    if (_requestDoing.IsFinished)
                    {
                        RequestsDone.Add(_requestDoing);
                        _requestDoing = null;
                    }
                }
            }
            return result;            
        }
        
        protected virtual int GetAndStoreResponse()
        {
            int result = 0;
            while (_rxData.Count > 0)
            {
                char[] buffer;
                if (_rxData.TryDequeue(out buffer))
                {
                    if (_responseDoing == null)
                        _responseDoing = new GrblResponse();
                    _responseDoing.SetContent(new string(buffer));
                    if (_responseDoing.IsFinished)
                    {
                        if (_requestDoing != null)
                        {
                            _responseDoing.SetRequest(_requestDoing);
                            if (_requestDoing.SetResponse(_responseDoing))
                            {
                                RequestsDone.Add(_requestDoing);
                                _requestDoing = null;
                            }
                        }
                        result++;
                        Responses.Add(_responseDoing);
                        _responseDoing = null;
                    }
                }
            }
            return result;
        }
        
        protected virtual bool CheckIfCancelRequest()
        {
            if (_requestDoing != null)
            {
                if (_requestCancelSource.Token.IsCancellationRequested)
                {
                    RequestsRejected.Add(_requestDoing);
                    _requestDoing = null;

                    if (PauseOnReject)
                    {
                        IsPaused = true;
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual void ThreadMainCleanup()
        {

            while (RequestsToDo.Count > 0)
                RequestsRejected.Add(RequestsToDo.Pop());
            if (_requestDoing != null)
                RequestsRejected.Add(_requestDoing);
            _requestDoing = null;
            if (_requestCancelSource != null)
                _requestCancelSource.Dispose();

            _runThread = null;
        }

        protected virtual void ThreadMain()
        {
            _requestCancelSource = new CancellationTokenSource();
            _threadCancelSource = new CancellationTokenSource();
            CancellationToken cancelToken = _threadCancelSource.Token;
            while (!cancelToken.IsCancellationRequested)
            {
                if (IsPaused)
                {
                    if (AllowSpecialWhenHalted)
                        GetAndSendSpecial();
                }
                else
                {
                    GetAndSendSpecial();
                    GetAndSendRequest();
                }
                GetAndStoreResponse();
                CheckIfCancelRequest();                
                Thread.Sleep(1);
            }
            ThreadMainCleanup();
        }    
    }
}
