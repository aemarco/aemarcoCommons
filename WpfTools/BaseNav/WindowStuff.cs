using aemarcoCommons.WpfTools.BaseModels;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace aemarcoCommons.WpfTools.BaseNav;

public abstract class BaseNavWindowViewModel : BaseViewModel //inherit this in window view model
{
    /// <summary>
    /// forward the window which uses that viewmodel
    /// </summary>
    /// <param name="window">window which uses this view model</param>
    protected BaseNavWindowViewModel(Window window)
    {
        Window = window;
        Window.DataContext = this;
    }

    //so that we can access the window belonging to this view model
    public Window Window { get; set; }




    //so that we can navigate
    public void ShowViewFor<T>() where T : INavViewModel
    {
        ShowViewFor(typeof(T));
    }
    // ReSharper disable once MemberCanBeProtected.Global
    public void ShowViewFor(Type type)
    {
        if (!typeof(INavViewModel).IsAssignableFrom(type))
            throw new Exception("Given type is not a NavViewModel");


        ViewViewModel = (INavViewModel)Resolve(type);
        //set reference so that navigation view models can access this window view model
        ViewViewModel.WindowViewModel = this;

        //Update View so navigation takes place
        View = ViewViewModel.View;


        OnPropertyChanged(nameof(ViewViewModel));
        OnPropertyChanged(nameof(View));
        OnPropertyChanged(nameof(Title));

        ((IMessenger)Resolve(typeof(IMessenger))).Send(new NavigationCompleted(type));
    }


    /// <summary>
    /// The viewmodel for the current view can be accessed here.
    /// </summary>
    public INavViewModel ViewViewModel { get; set; }

    /// <summary>
    /// Bind this to get the navigation view
    /// </summary>
    public INavView View { get; set; }

    /// <summary>
    /// Bind this to get the Title
    /// </summary>
    public virtual string Title => ViewViewModel?.Title ?? Window.Title;


    internal static IServiceProvider ServiceProvider { get; set; }


    /// <summary>
    /// Resolve requested INavViewModel. Override if DiExtension is not used!
    /// </summary>
    /// <returns>requested view model</returns>
    protected virtual object Resolve(Type type)
    {
        if (ServiceProvider is null)
        {
            throw new Exception("""
                ServiceProvider unavailable.
                Either
                    - Setup ServiceProvider like this IServiceProvider.SetupServiceProviderForWpfTools();
                    - override object Resolve(Type type) to resolve objects
                """);
        }

        var result = ServiceProvider.GetRequiredService(type);
        return result;
    }

    protected override void Close()
    {
        Window?.Close();
    }
}

public record NavigationCompleted(Type ViewModelType);