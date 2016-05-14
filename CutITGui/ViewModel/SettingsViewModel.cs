using CutIT.GRBL;
using CutIT.Utility;
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
        List<Guid> _settings;
        GrblSettings _grblSettings = null;
        string _settingValue = "";
        public string SettingValue { get { return _settingValue; } set { Set(ref _settingValue, value); } }        
          
        public SettingsViewModel()
        {
            Visibility = System.Windows.Visibility.Hidden;
            _settings = new List<Guid>();
            _grblSettings = ViewModelLocator.GrblSettings;
            _grblSettings.PropertyChanged += _grblSettings_PropertyChanged;
        }

        private void _grblSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {  
        }
    }
}
