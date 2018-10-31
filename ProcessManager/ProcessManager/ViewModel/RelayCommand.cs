using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProcessManager.ViewModel
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Func<bool> canExecuteFunc;
        private Action executeAction;
        private Action<object> execute;

        public RelayCommand(Action executeAction) : this(executeAction, () => true)
        {
        }

        public RelayCommand(Action executeAction, Func<bool> canExecuteFunc)
        {
            this.executeAction = executeAction;
            this.canExecuteFunc = canExecuteFunc;
        }

        public RelayCommand(Action<object> execute) : this(execute,()=>true)
        {
        }

        public RelayCommand(Action<object> execute, Func<bool> canExecuteFunc)
        {
            this.execute = execute;
            this.canExecuteFunc = canExecuteFunc;
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteFunc();
        }

        public void Execute(object parameter)
        {
            executeAction?.Invoke();
            execute?.Invoke(parameter);
        }
    }
}
