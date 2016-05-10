using CutIT.GRBL;
using CutIT.Messages;
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
    public class ConsoleViewModel:ViewModelBase
    {
        ICommand _inputCommand = null;
        string _consoleInput = "";
        string _consoleOutput = "";
        TcpGrblClient _tcpGrblClient;
        MessageViewModel _messageViewModel;

        public string ConsoleInput {
            get { return _consoleInput; }
            set { Set("ConsoleInput",ref _consoleInput, value); }
        }

        public string ConsoleOutput
        {
            get { return _consoleOutput; }
            set { Set("ConsoleOutput", ref _consoleOutput, value); }
        }

        public ConsoleViewModel()
        {            
            _tcpGrblClient = ViewModelLocator.TcpGrblClient;
            _messageViewModel = ViewModelLocator.MessageViewModel;
        }        

        public ICommand InputCommand { get { if (_inputCommand == null) _inputCommand = new RelayCommand(DoInputCommand); return _inputCommand; } }

        public void DoInputCommand()
        {
            _tcpGrblClient.Add(_consoleInput);            
            ConsoleInput = "";
        }
    }
}
