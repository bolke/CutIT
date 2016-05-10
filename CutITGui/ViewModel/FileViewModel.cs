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
        void DoOpenFileCommand()
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
        }
    }
}
