using aemarcoCommons.WpfTools.BaseModels;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

#nullable enable
namespace aemarcoCommons.WpfTools.Commands;

public class ShowWindowCommand<T> : DelegateCommand
    where T : IWindow
{
    public ShowWindowCommand(ILifetimeScope lifetimeScope) =>
        CommandAction = _ => Show(lifetimeScope.Resolve<T>);

    public ShowWindowCommand(IServiceProvider serviceProvider) =>
        CommandAction = _ => Show(serviceProvider.GetRequiredService<T>);

    private void Show(Func<T?> getWindow)
    {
        foreach (Window win in Application.Current.Windows)
        {
            if (win is T)
            {
                win.WindowState = WindowState.Normal;
                win.Activate();
                return;
            }
        }
        (getWindow() as Window)?.Show();
    }

}
