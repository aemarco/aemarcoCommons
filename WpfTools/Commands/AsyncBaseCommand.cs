using System;
using System.Threading.Tasks;
using System.Windows.Input;
#nullable enable

namespace aemarcoCommons.WpfTools.Commands;

public abstract class AsyncBaseCommand : IAsyncCommand
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


    public async void Execute(object? parameter)
    {
        await ExecuteAsync(parameter);
    }

    public abstract Task ExecuteAsync(object? parameter);
}