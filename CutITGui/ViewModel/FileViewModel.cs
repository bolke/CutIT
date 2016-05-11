using CutIT.GRBL;
using CutITGui.Messages;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CutITGui.ViewModel
{
    public class FileViewModel:TabViewModel
    {
        string _fileContent = "";
        
        public string FileContent
        {
            get { return _fileContent; }
            set { Set("FileContent", ref _fileContent, value); }
        }

        public FileViewModel()
        {
            Visibility = Visibility.Hidden;
        }

        ICommand _openFileCommand;
        public ICommand OpenFileCommand { get { if (_openFileCommand == null) _openFileCommand = new RelayCommand(DoOpenFileCommand); return _openFileCommand; } }

        async void DoOpenFileCommand()
        {
            await Task.Run(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = Environment.CurrentDirectory;
                openFileDialog.Filter = "GCode files (*.g *.nc)|*.g;*.nc|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    FileContent = "";
                    FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open);
                    StreamReader streamReader = new StreamReader(fileStream);
                    while (!streamReader.EndOfStream)
                    {
                        FileContent += streamReader.ReadLine() + "\n";
                    }
                    fileStream.Close();
                }
            });
        }

        ICommand _executeFileCommand;
        public ICommand ExecuteFileCommand { get { if (_executeFileCommand == null) _executeFileCommand = new RelayCommand(DoExecuteFileCommand); return _executeFileCommand; } }

        async void DoExecuteFileCommand()
        {
            await Task.Run(() =>
            {
                string[] lines = FileContent.Split('\n');
                TcpGrblClient tcpGrblClient = ViewModelLocator.TcpGrblClient;
                foreach (string line in lines)
                {
                    UserRequest request = new UserRequest();
                    request.SetContent(line);
                    tcpGrblClient.Add(request);
                }
            });
        }
    }
}
