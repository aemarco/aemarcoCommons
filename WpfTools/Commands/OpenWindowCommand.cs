using aemarcoCommons.WpfTools.BaseModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace aemarcoCommons.WpfTools.Commands;

public class OpenWindowCommand<T> : DelegateCommand
    where T : IWindow
{
    public OpenWindowCommand(IServiceProvider serviceProvider)
    {
        CommandAction = _ =>
        {
            serviceProvider.GetRequiredService<T>().Show();
        };
    }
}