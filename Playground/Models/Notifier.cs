using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Models
{
    /// <summary>
    /// Class to handle property changed events
    /// </summary>
    public class Notifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// The Notify Property Changed event handler. Notifies the UI that a property has changed
        /// </summary>
        /// <param name="name">The name of the property, defaults to empty string</param>
        public void OnNotifyPropertyChanged([CallerMemberName] string name = "")//By using CallerMemberName no need to hard code property names, reduces errors! Obtains method/property name of caller to the method
        {
            // the ? means if PropertyChanged is null it wont invoke.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
