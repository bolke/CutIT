using CutIT.GRBL;
using CutIT.Messages;
using CutIT.Utility;
using CutITGui.Messages;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutITGui.ViewModel
{
    public class MessageViewModel
    {
        GrblClient _grblClient = null;
        ConcurrentObservableCollection<GrblRequest> _requestsDone;
        ConcurrentObservableCollection<GrblRequest> _requestsToDo;
        ConcurrentObservableCollection<GrblRequest> _requestsRejected;
        ConcurrentObservableCollection<GrblRequest> _specialRequests;
        ConcurrentObservableCollection<GrblResponse> _responses;

        ConsoleViewModel _consoleViewModel;

        ConsoleViewModel ConsoleViewModel { get { if (_consoleViewModel == null) _consoleViewModel = ViewModelLocator.ConsoleViewModel; return _consoleViewModel; } }

        public MessageViewModel()
        {
            _grblClient = ViewModelLocator.GrblClient;

            _requestsToDo = _grblClient.RequestsToDo;
            _requestsDone = _grblClient.RequestsDone;
            _requestsRejected = _grblClient.RequestsRejected;
            _specialRequests = _grblClient.SpecialRequests;
            _responses = _grblClient.Responses;
            
            _grblClient.RequestsDone.CollectionChanged += RequestsDone_CollectionChanged;
            _grblClient.RequestsRejected.CollectionChanged += RequestsRejected_CollectionChanged;
            _grblClient.Responses.CollectionChanged += Responses_CollectionChanged;
        }
        
        private void Responses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(GrblResponse response in e.NewItems)
                {
                    if (response.Request != null && (response.Request as GuiRequest) == null)
                    {
                        ConsoleViewModel.ConsoleOutput += response.Content + "\n";
                    }
                }
            }
        }

        private void RequestsRejected_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {            
            //rejected requests, ready for inspection
        }

        private void RequestsDone_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (GrblRequest request in e.NewItems)
                {
                    if(!(request is GuiRequest))
                        ConsoleViewModel.ConsoleOutput += request.Content + "\n";
                }
            }
        }
    }
}
