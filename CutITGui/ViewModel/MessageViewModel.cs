using CutIT.GRBL;
using CutIT.Messages;
using CutIT.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutITGui.ViewModel
{
    public class MessageViewModel
    {
        TcpGrblClient _tcpGrblClient;
        ConcurrentObservableCollection<GrblRequest> _requestsDone;
        ConcurrentObservableCollection<GrblRequest> _requestsToDo;
        ConcurrentObservableCollection<GrblRequest> _requestsRejected;
        ConcurrentObservableCollection<GrblRequest> _specialRequests;
        ConcurrentObservableCollection<GrblResponse> _responses;

        public MessageViewModel()
        {
            _tcpGrblClient = ViewModelLocator.TcpGrblClient;

            _requestsToDo = _tcpGrblClient.RequestsToDo;
            _requestsDone = _tcpGrblClient.RequestsDone;
            _requestsRejected = _tcpGrblClient.RequestsRejected;
            _specialRequests = _tcpGrblClient.SpecialRequests;
            _responses = _tcpGrblClient.Responses;
            
            _tcpGrblClient.RequestsDone.CollectionChanged += RequestsDone_CollectionChanged;
            _tcpGrblClient.RequestsRejected.CollectionChanged += RequestsRejected_CollectionChanged;
            _tcpGrblClient.Responses.CollectionChanged += Responses_CollectionChanged;
        }
        
        private void Responses_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //responses ready to show 
        }

        private void RequestsRejected_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {            
            //rejected requests, ready for inspection
        }

        private void RequestsDone_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {            
            //finished requests
        }
    }
}
