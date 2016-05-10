using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CutITGui.ViewModel
{
    public class TabViewModel: ViewModelBase
    {
        Visibility _visibility;
        public Visibility Visibility { get { return _visibility; } set { Set("Visibility", ref _visibility, value); } }
        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }
    }
}
