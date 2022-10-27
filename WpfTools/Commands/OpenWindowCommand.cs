using aemarcoCommons.WpfTools.BaseModels;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace aemarcoCommons.WpfTools.Commands;

public class OpenWindowCommand<T> : DelegateCommand where T : IWindow
{
    public OpenWindowCommand(ILifetimeScope lifetimeScope)
    {
        CommandAction = _ =>
        {
            var window = lifetimeScope.Resolve<T>();
            window.Show();
        };

    }

    public OpenWindowCommand(IServiceProvider serviceProvider)
    {
        CommandAction = _ =>
        {
            var window = serviceProvider.GetRequiredService<T>();
            window.Show();
        };

    }
}