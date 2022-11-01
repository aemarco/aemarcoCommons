using aemarcoCommons.Extensions.NetworkExtensions;
using aemarcoCommons.WpfTools.Commands;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;

namespace aemarcoCommons.WpfTools.BaseModels;

// ReSharper disable once PartialTypeWithSinglePart
public partial class BaseViewModel : BaseNotifier, IBaseViewModel
{
    [RelayCommand]
    // ReSharper disable once UnusedMember.Local
    protected virtual void NavigateToUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new Exception("Must provide a url");
        new Uri(url).OpenInBrowser();
    }



    public virtual DelegateCommand CloseCommand { get; } = new();


    [RelayCommand]
    protected virtual void ExitApplication()
        => Application.Current.Shutdown();

}