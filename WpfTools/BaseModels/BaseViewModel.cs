using aemarcoCommons.Extensions.NetworkExtensions;
using aemarcoCommons.WpfTools.Commands;
using System;
using System.Windows.Input;

namespace aemarcoCommons.WpfTools.BaseModels;

public class BaseViewModel : BaseNotifier, IBaseViewModel
{
    public virtual DelegateCommand CloseCommand { get; } = new DelegateCommand();

    public virtual ICommand NavigateToUrlCommand =>
        new DelegateCommand
        {
            CommandAction = x =>
            {
                if (x is not string url)
                    return;
                new Uri(url).OpenInBrowser();
            }
        };

}