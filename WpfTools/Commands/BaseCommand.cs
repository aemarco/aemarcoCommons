using System;
using System.Windows.Input;
#nullable enable

namespace aemarcoCommons.WpfTools.Commands;

public abstract class BaseCommand : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
    protected virtual void OnCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
        
    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }
        
    public abstract void Execute(object? parameter);

}