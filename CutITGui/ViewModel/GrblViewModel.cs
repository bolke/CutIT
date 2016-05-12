using CutIT.GRBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutITGui.ViewModel
{
    public class GrblViewModel:TabViewModel
    {
        GrblClient _grblClient = null;
        
        public GrblViewModel(GrblClient grblClient)
        {
            _grblClient = grblClient;
        }

    }
}
