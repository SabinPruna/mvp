using System;
using System.Windows.Input;

namespace MatchGame.Commands {
    public class RelayCommandObject : ICommand {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;

        #region Constructors

        public RelayCommandObject(Action<object> execute, Func<object, bool> canExecute = null) {
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter) {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter) {
            _execute(parameter);
        }

        #endregion
    }
}