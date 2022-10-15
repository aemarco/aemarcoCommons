using System;
using System.Threading.Tasks;

#nullable enable

namespace aemarcoCommons.WpfTools.Commands;

public class AsyncDelegateCommand : AsyncBaseCommand
{
    public Func<object?, bool>? CanExecuteFunc { get; set; }
    public override bool CanExecute(object? parameter)
    {
        return CanExecuteFunc == null || CanExecuteFunc(parameter);
    }


    public Func<object?, Task>? CommandAction { get; set; }
    public override Task ExecuteAsync(object? parameter)
    {
        return CommandAction is null
            ? Task.CompletedTask
            : CommandAction(parameter);
    }
}