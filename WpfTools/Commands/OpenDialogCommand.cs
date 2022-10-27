using aemarcoCommons.WpfTools.BaseModels;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace aemarcoCommons.WpfTools.Commands;

public class OpenDialogCommand<T> : DelegateCommand where T : IWindow
{
    public OpenDialogCommand(ILifetimeScope lifetimeScope)
    {

        CommandAction = _ =>
        {
            var window = lifetimeScope.Resolve<T>();
            window.ShowDialog();
        };
    }

    public OpenDialogCommand(IServiceProvider serviceProvider)
    {


        CommandAction = _ =>
        {
            var window = serviceProvider.GetRequiredService<T>();
            window.ShowDialog();
        };
    }
}