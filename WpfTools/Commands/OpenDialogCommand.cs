using aemarcoCommons.WpfTools.BaseModels;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace aemarcoCommons.WpfTools.Commands;

public class OpenDialogCommand<T> : DelegateCommand
    where T : IWindow
{
    public OpenDialogCommand(ILifetimeScope lifetimeScope) =>
        CommandAction = _ => (lifetimeScope.Resolve<T>() as Window)?.ShowDialog();
    public OpenDialogCommand(IServiceProvider serviceProvider) =>
        CommandAction = _ => (serviceProvider.GetRequiredService<T>() as Window)?.ShowDialog();
}