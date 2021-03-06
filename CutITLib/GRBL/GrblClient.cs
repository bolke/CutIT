﻿using CutIT.Connection;
using CutIT.Messages;
using CutIT.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutIT.GRBL
{
    public class GrblClient
    {
        StreamClient _streamClient;
        MessagePacker _messagePacker;

        public bool IsRunning { get { return _messagePacker.IsRunning; } }
        public bool IsPaused { get { return _messagePacker.IsPaused; } }
        public ConcurrentObservableCollection<GrblResponse> Responses { get { return _messagePacker.Responses; } }
        public ConcurrentObservableCollection<GrblRequest> RequestsToDo { get { return _messagePacker.RequestsToDo; } }
        public ConcurrentObservableCollection<GrblRequest> RequestsDone { get { return _messagePacker.RequestsDone; } }
        public ConcurrentObservableCollection<GrblRequest> SpecialRequests { get { return _messagePacker.SpecialRequests; } }
        public ConcurrentObservableCollection<GrblRequest> RequestsRejected { get { return _messagePacker.RequestsRejected; } }
        public GrblClient()
        {
            _streamClient = new StreamClient();
            _streamClient.ReadLines = true;
            _messagePacker = new MessagePacker(_streamClient.RxData, _streamClient.TxData);
            _messagePacker.SaveRejectedRequests = true;
        }

        public GrblClient(Stream stream) : this()
        {
            Start(stream);
        }

        public bool Start(Stream stream)
        {
            if (!IsRunning)
            {
                _messagePacker.Start();
                return _streamClient.Start(stream);
            }
            return false;
        }

        public bool Add(string request)
        {
            if (IsRunning)
            {
                return _messagePacker.Add(request);
            }
            return false;
        }

        public bool Add(GrblRequest request)
        {
            if (IsRunning)
            {
                return _messagePacker.Add(request);
            }
            return false;
        }

        public virtual void Stop()
        {
            _streamClient.Stop();
            _messagePacker.Stop();
        }
    }
}
