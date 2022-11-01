using aemarcoCommons.WpfTools.BaseModels;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace aemarcoCommons.WpfTools.Commands;

public class OpenWindowCommand<T> : DelegateCommand
    where T : IWindow
{
    public OpenWindowCommand(ILifetimeScope lifetimeScope) =>
        CommandAction = _ => (lifetimeScope.Resolve<T>() as Window)?.Show();
    public OpenWindowCommand(IServiceProvider serviceProvider) =>
        CommandAction = _ => (serviceProvider.GetRequiredService<T>() as Window)?.Show();
}