using aemarcoCommons.WpfTools.BaseModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

#nullable enable
namespace aemarcoCommons.WpfTools.Commands;

public class ShowWindowCommand<T> : DelegateCommand
    where T : IWindow
{
    public ShowWindowCommand(IServiceProvider serviceProvider)
    {
        CommandAction = _ =>
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
            serviceProvider.GetRequiredService<T>().Show();
        };
    }
}
