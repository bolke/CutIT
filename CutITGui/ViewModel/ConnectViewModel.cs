using CutIT.GRBL;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CutITGui.ViewModel
{
    public class ConnectViewModel : ViewModelBase
    {
        TcpGrblClient _tcpGrblClient;
        string _address = "";
        string _port = "";

        public bool IsConnected
        {
            get { return _tcpGrblClient.IsConnected; }
        }

        public ConnectViewModel()
        {
            _tcpGrblClient = ViewModelLocator.TcpGrblClient;
        }

        public string Address { get { return _address; } set { if(!IsConnected) Set("Address", ref _address, value); } }
        public string Port { get { return _port; } set { if(!IsConnected) Set("Port", ref _port, value); } }

        ICommand _connectCommand;
        public ICommand ConnectCommand { get { if(_connectCommand==null){ _connectCommand = new RelayCommand(DoConnectCommand); } return _connectCommand; } }

        private void DoConnectCommand()
        {
            if (_tcpGrblClient.IsConnected)
                _tcpGrblClient.Stop();
            else
            {
                int port = 0;
                if (int.TryParse(_port, out port))
                {
                    _tcpGrblClient.Start(_address, port);
                }                
            }
            RaiseAllProperties();
        }

        private void RaiseAllProperties()
        {
            RaisePropertyChanged("IsConnected");
            RaisePropertyChanged("ConnectedText");
        }

        public string ConnectedText
        {
            get { return IsConnected ? "Disconnect" : "Connect"; }
        }
    }
}
