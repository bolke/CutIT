using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CutITGui.ViewModel
{
    public class FileViewModel:TabViewModel
    {
        string _fileContent = "";
        public string FileContent { get { return _fileContent; } set { Set("FileContent", ref _fileContent, value); } }

        public FileViewModel()
        {
            Visibility = Visibility.Hidden;
        }
    }
}
