using System;
using System.Windows.Input;
#nullable enable

namespace aemarcoCommons.WpfTools.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public abstract void Execute(object? parameter);
        public virtual bool CanExecute(object? parameter)
        {
            return true;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
