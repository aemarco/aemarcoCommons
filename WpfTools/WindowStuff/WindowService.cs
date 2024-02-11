using aemarcoCommons.WpfTools.BaseModels;
using aemarcoCommons.WpfTools.Dialogs;
using System;
using System.Windows;

namespace aemarcoCommons.WpfTools.WindowStuff;

public class WindowService : IWindowService
{

    private readonly IServiceProvider _serviceProvider;
    public WindowService() { }
    public WindowService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    //IWindowService
    public void Show<TViewModel>()
        where TViewModel : BaseViewModel
    {
        var viewModelType = typeof(TViewModel);
        var viewModel = GetViewModel(viewModelType);
        var viewType = GetViewType(viewModelType);
        ShowInternal(viewModel, viewType);
    }

    public bool? ShowDialog(DialogViewModel viewModel)
    {
        var viewModelType = viewModel.GetType();
        var viewType = GetViewType(viewModelType);
        return ShowInternal(viewModel, viewType);
    }
    public bool? ShowDialog<TViewModel>()
        where TViewModel : DialogViewModel
    {
        var viewModelType = typeof(TViewModel);
        var viewModel = GetViewModel(viewModelType);
        var viewType = GetViewType(viewModelType);
        return ShowInternal(viewModel, viewType);
    }



    //viewmodel
    private object GetViewModel(Type viewModelType)
    {
        var result = _serviceProvider?.GetService(viewModelType)
                     ?? Activator.CreateInstance(viewModelType)
                     ?? throw new Exception($"ViewModel {viewModelType.FullName} could not be created.");
        return result;
    }

    //view
    private static Type GetViewType(Type viewModelType)
    {
        if (!IWindowService.Mappings.TryGetValue(viewModelType, out var result))
            throw new Exception($"Window for view model {viewModelType.FullName} not registered.");
        return result;
    }


    //show
    private static bool? ShowInternal(object viewModel, Type viewType)
    {
        if (viewModel is DialogViewModel dialog)
            return ShowDialogInternal(dialog, viewType);

        ShowWindowInternal(viewModel, viewType);
        return null;
    }
    private static void ShowWindowInternal(object viewModel, Type viewType)
    {
        foreach (Window win in Application.Current.Windows)
        {
            if (viewType.IsInstanceOfType(win))
            {
                win.WindowState = WindowState.Normal;
                win.Activate();
                return;
            }
        }

        var window = (Window)Activator.CreateInstance(viewType)
                     ?? throw new Exception($"Window {viewType.FullName} could not be created.");
        window.DataContext = viewModel;
        window.Show();
    }
    private static bool? ShowDialogInternal(DialogViewModel viewModel, Type viewType)
    {
        var dialog = new DialogWindow
        {
            DataContext = viewModel,
            Content = Activator.CreateInstance(viewType)
        };
        viewModel.CloseDialog += CloseEventHandler;

        var result = dialog.ShowDialog();
        return result;

        void CloseEventHandler(object s, bool? e)
        {
            dialog.DialogResult = e;
            viewModel.CloseDialog -= CloseEventHandler;
        }
    }



}