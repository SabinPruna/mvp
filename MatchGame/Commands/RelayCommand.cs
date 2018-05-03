using System;
using System.Windows.Input;

namespace MatchGame.Commands {
    public class RelayCommand : ICommand {
        private readonly Func<bool> _canExecuteEvaluator;
        private readonly Action _methodToExecute;

        #region Constructors

        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator) {
            _methodToExecute = methodToExecute;
            _canExecuteEvaluator = canExecuteEvaluator;
        }

        public RelayCommand(Action methodToExecute)
            : this(methodToExecute, null) { }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter) {
            if (_canExecuteEvaluator == null) {
                return true;
            }

            bool result = _canExecuteEvaluator.Invoke();
            return result;
        }

        public event EventHandler CanExecuteChanged {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter) {
            _methodToExecute.Invoke();
        }

        #endregion
    }
}