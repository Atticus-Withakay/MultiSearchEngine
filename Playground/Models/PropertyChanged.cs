using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Models
{
    class PropertyChanged: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnNotifyPropertyChanged([CallerMemberName] string name = "")//By using CallerMemberName no need to hard code property names, reduces errors! Obtains method/property name of caller to the method
        {
            // the ? means if PropertyChanged is null it wont invoke.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
