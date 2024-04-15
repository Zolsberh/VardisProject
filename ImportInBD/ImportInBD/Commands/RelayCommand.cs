using System;
using System.Windows.Input;

namespace ImportInBD.Commands
{
    class RelayCommand : ICommand
    {

        private Action<object> _execute;
        private Func<object, bool> _executeFunc;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> executeFunc = null)
        {
            _execute = execute;
            _executeFunc = executeFunc;
        }

        public bool CanExecute(object? parameter)
        {
            return _executeFunc == null || _executeFunc(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
