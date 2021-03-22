using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Playground.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region ICommand Members
        
        Action<object> _action = null;
        Predicate<object> _predicate = null;

        public DelegateCommand(Action<object> action, Predicate<object> predicate)
        {
            _action = action;
            _predicate = predicate;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _predicate(parameter);
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }
        
        #endregion
    }
}
