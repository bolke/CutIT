using CutIT.GRBL;
using CutIT.Utility;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutITGui.ViewModel
{
   public class GrblSettingsViewModel: ViewModelBase
    {
        public GrblStateEnum GrblState { get { return _grblSettings.GrblState; } }
        public Coordinate MachinePosition { get { return _grblSettings.MachinePosition; } }
        public Coordinate WorkPosition { get { return _grblSettings.WorkPosition; } }
        public bool LastProbeSuccess { get { return _grblSettings.LastProbeSuccess; } }
        public double ToolLengthOffset { get { return _grblSettings.ToolLengthOffset; } }
        public string StartupBlock1 { get { return _grblSettings.StartupBlock1; } }
        public string StartupBlock2 { get { return _grblSettings.StartupBlock2; } }
        public string BuildInfo { get { return _grblSettings.BuildInfo; } }
        public List<Tuple<string,string>> Settings { get; protected set; }
        public List<Tuple<string,Coordinate>> GCodeParameters { get; protected set; }
        GrblSettings _grblSettings { get { return ViewModelLocator.GrblClient.GrblSettings; } } 

        public GrblSettingsViewModel()
        {
            Settings = new List<Tuple<string, string>>();
            GCodeParameters = new List<Tuple<string, Coordinate>>();

            foreach (var element in _grblSettings.Settings)
            {
                Settings.Add(new Tuple<string,string>(element.Key, element.Value));
            }

            foreach(var element in _grblSettings.GCodeParameters)
            {
                GCodeParameters.Add(new Tuple<string, Coordinate>(element.Key, new Coordinate(element.Value)));
            }
            _grblSettings.PropertyChanged += _grblSettings_PropertyChanged;
        }

        private void _grblSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }
    }
}
