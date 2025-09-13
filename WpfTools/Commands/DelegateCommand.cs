using System;


namespace aemarcoCommons.WpfTools.Commands;

/// <summary>
/// Simplistic delegate command .
/// </summary>
public class DelegateCommand : BaseCommand
{
    public Func<object?, bool>? CanExecuteFunc { get; set; }
    public override bool CanExecute(object? parameter)
    {
        return CanExecuteFunc == null || CanExecuteFunc(parameter);
    }

    public Action<object?>? CommandAction { get; set; }
    public override void Execute(object? parameter)
    {
        CommandAction?.Invoke(parameter);
    }
}