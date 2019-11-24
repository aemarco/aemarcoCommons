using System;
using System.Windows.Input;

namespace WpfTools.Commands
{
    /// <summary>
    /// Simplistic delegate command .
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Action<object> ObjectCommandAction { get; set; }

        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            if (CommandAction != null)
                CommandAction();
            else
                ObjectCommandAction(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
