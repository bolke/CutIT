using CutIT.GRBL;
using CutIT.Messages;
using CutITGui.Messages;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CutITGui.ViewModel
{
    public class ActiveStatusViewModel:ViewModelBase
    {
        GrblRequest _statusRequest = null;
        Timer _statusTimer;
        GrblSettings _grblSettings;
        public ActiveStatusViewModel()
        {
            _grblSettings = ViewModelLocator.GrblSettings;
            _statusTimer = new Timer();
            _statusTimer.Interval = 1000;
            _statusTimer.Elapsed += _statusTimer_Elapsed;
            _statusTimer.AutoReset = false;
            _statusTimer.Start();
            RaiseAllProperties();
        }

        private void _statusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ViewModelLocator.TcpGrblClient.IsConnected)
            {
                if(_statusRequest == null)
                {
                    _statusRequest = new GuiRequest();
                    _statusRequest.SetContent("?");
                    if (!ViewModelLocator.TcpGrblClient.Add(_statusRequest))
                        _statusRequest = null;
                }else if (_statusRequest.IsFinished)
                {
                    ViewModelLocator.GrblSettings.Parse(_statusRequest.Response);
                    RaiseAllProperties();
                    _statusRequest = null;
                }
            }
            _statusTimer.Start();
        }

        private void RaiseAllProperties()
        {
            RaisePropertyChanged("MPosX");
            RaisePropertyChanged("MPosY");
            RaisePropertyChanged("MPosZ");
            RaisePropertyChanged("WPosX");
            RaisePropertyChanged("WPosY");
            RaisePropertyChanged("WPosZ");
            RaisePropertyChanged("Status");
        }

        public string MPosX { get { return _grblSettings.MachinePosition.X.ToString("000.00"); } }
        public string MPosY { get { return _grblSettings.MachinePosition.Y.ToString("000.00"); } }
        public string MPosZ { get { return _grblSettings.MachinePosition.Z.ToString("000.00"); } }

        public string WPosX { get { return _grblSettings.WorkPosition.X.ToString("000.00"); } }
        public string WPosY { get { return _grblSettings.WorkPosition.Y.ToString("000.00"); } }
        public string WPosZ { get { return _grblSettings.WorkPosition.Z.ToString("000.00"); } }

        public string Status { get { return _grblSettings.GrblState.ToString(); } }
    }
}
