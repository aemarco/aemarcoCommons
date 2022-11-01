using System;
using System.Threading.Tasks;
using System.Windows.Input;

#nullable enable

namespace aemarcoCommons.WpfTools.Commands;

public class AsyncDelegateCommand : ICommand
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



    public Func<object?, bool>? CanExecuteFunc { get; init; }
    public bool CanExecute(object? parameter)
    {
        return CanExecuteFunc is null || CanExecuteFunc(parameter);
    }


    public Func<object?, Task>? CommandAction { get; init; }
    public async void Execute(object? parameter)
    {
        if (CommandAction is not null)
            await CommandAction(parameter);
    }


}