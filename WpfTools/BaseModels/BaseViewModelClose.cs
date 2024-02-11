using aemarcoCommons.WpfTools.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

#nullable enable

namespace aemarcoCommons.WpfTools.BaseModels;
public partial class BaseViewModel : ICloseWindow
{

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CloseCommand))]
    private Action? _closeAction;

    [RelayCommand(CanExecute = nameof(CanClose))]
    protected virtual void Close()
    {
        CloseAction?.Invoke();
    }

    [ObservableProperty]
    private bool _canClose = true;

}



