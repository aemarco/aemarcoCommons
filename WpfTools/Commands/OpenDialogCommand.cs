using aemarcoCommons.WpfTools.BaseModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace aemarcoCommons.WpfTools.Commands;

public class OpenDialogCommand<T> : DelegateCommand
    where T : IWindow
{
    public OpenDialogCommand(IServiceProvider serviceProvider)
    {
        CommandAction = _ =>
        {
            serviceProvider.GetRequiredService<T>().ShowDialog();
        };
    }
}