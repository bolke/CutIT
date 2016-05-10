using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CutITGui.ViewModel
{
    public class ScreenTabViewModel
    {
        ICommand _showConsoleCommand = null;
        ICommand _showFileCommand = null;
        ICommand _showSettingsCommand = null;
        ICommand _showGrblCommand = null;

        public ICommand ShowConsoleCommand { get { if (_showConsoleCommand == null) _showConsoleCommand = new RelayCommand(DoShowConsole); return _showConsoleCommand; } }
        public ICommand ShowFileCommand { get { if (_showFileCommand == null) _showFileCommand = new RelayCommand(DoShowFile); return _showFileCommand; } }
        public ICommand ShowSettingsCommand { get { if (_showSettingsCommand == null) _showSettingsCommand = new RelayCommand(DoShowSettings); return _showSettingsCommand; } }
        public ICommand ShowGrblCommand { get { if (_showGrblCommand == null) _showGrblCommand = new RelayCommand(DoShowGrbl); return _showGrblCommand; } }

        private void DoShowConsole()
        {
            ViewModelLocator.ConsoleViewModel.Show();
        }

        private void DoShowFile()
        {
            ViewModelLocator.ConsoleViewModel.Hide();
        }

        private void DoShowSettings()
        {
            ViewModelLocator.ConsoleViewModel.Hide();
        }

        private void DoShowGrbl()
        {
            ViewModelLocator.ConsoleViewModel.Hide();
        }
    
    }
}
