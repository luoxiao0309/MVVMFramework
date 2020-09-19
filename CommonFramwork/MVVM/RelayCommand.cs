using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CommonFramework.MVVM
{
    public class RelayCommand : ICommand
    {
        private Action<object> executeAction;
        private Func<object, bool> canExecuteFunc;
        
        public RelayCommand(Action<object> execute) : this(execute, null)
        {

        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            if (execute == null)
            {
                return;
            }
            executeAction = execute;
            canExecuteFunc = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecuteFunc == null)
            {
                return true;
            }
            return canExecuteFunc(parameter);
        }
        public void Execute(object parameter)
        {
            if (executeAction == null)
            {
                return;
            }
            executeAction(parameter);
        }

        //public event EventHandler<object> ViewChanged;

        //protected virtual void OnViewChanged(object obj)
        //{
        //    this.ViewChanged?.Invoke(this, obj);
        //}

        //public void AddHandler(EventHandler<EventArgs> handler)
        //{
        //    WeakEventManager<ViewModelBase, EventArgs>.AddHandler(this, nameof(this.ViewChanged), handler);
        //}
    }
}
