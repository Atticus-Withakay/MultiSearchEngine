using System;
using System.Windows.Input;

namespace Playground.Models
{
    /// <summary>
    /// Class to handle ICommand interface, bundles the work into a constructor.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region ICommand Members
        
        Action<object> _action = null;
        Predicate<object> _predicate = null;

        #endregion

        #region Constructor
        public DelegateCommand(Action<object> action, Predicate<object> predicate)
        {
            _action = action;
            _predicate = predicate;
        }
        #endregion

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
        
    }
}
