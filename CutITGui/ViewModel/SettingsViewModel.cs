using CutIT.GRBL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutITGui.ViewModel
{
    public class SettingsViewModel : TabViewModel
    {
        GrblSettings _grblSettings = null;
        string _settingValue = "";
        public string SettingValue { get { return _settingValue; } set { Set(ref _settingValue, value); } }
                
        public SettingsViewModel()
        {
            _grblSettings = ViewModelLocator.GrblSettings;
            _grblSettings.PropertyChanged += _grblSettings_PropertyChanged;
        }

        private void _grblSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }
    }
}
