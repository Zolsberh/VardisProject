using System;
using System.Windows.Input;

namespace ImportInBD
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
            this._execute = execute;
            this._executeFunc = executeFunc;
        }

        public bool CanExecute(object? parameter)
        {
            return this._executeFunc == null || this._executeFunc(parameter);
        }

        public void Execute(object? parameter)
        {
            this._execute(parameter);
        }
    }
}
