using System;
using System.Windows.Input;

namespace Match.Commands {
    public class RelayCommand : ICommand {
        private readonly Action<object> _commandTask;

        #region Constructors

        public RelayCommand(Action<object> commandTask) {
            _commandTask = commandTask;
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter) {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter) {
            _commandTask(parameter);
        }

        #endregion
    }
}