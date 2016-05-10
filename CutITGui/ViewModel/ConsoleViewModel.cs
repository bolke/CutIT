using CutIT.GRBL;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutITGui.ViewModel
{
    public class ConsoleViewModel:ViewModelBase
    {
        TcpGrblClient _tcpGrblClient;

        public ConsoleViewModel()
        {            
            _tcpGrblClient = ViewModelLocator.TcpGrblClient;
        }
    }
}
