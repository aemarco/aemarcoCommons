using aemarcoCommons.Extensions.NetworkExtensions;
using aemarcoCommons.WpfTools.Commands;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;

namespace aemarcoCommons.WpfTools.BaseModels;

// ReSharper disable once PartialTypeWithSinglePart
public partial class BaseViewModel : BaseNotifier, IBaseViewModel
{
    [RelayCommand(CanExecute = nameof(CanNavigateToUrl))]
    // ReSharper disable once UnusedMember.Local
    protected virtual void NavigateToUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new Exception("Must provide a url");
        new Uri(url).OpenInBrowser();
    }

    protected virtual bool CanNavigateToUrl(string url)
    {
        return !string.IsNullOrWhiteSpace(url) &&
               Uri.TryCreate(url, UriKind.Absolute, out _);
    }




    public virtual DelegateCommand CloseCommand { get; } = new();


    [RelayCommand]
    protected virtual void ExitApplication()
        => Application.Current.Shutdown();

}