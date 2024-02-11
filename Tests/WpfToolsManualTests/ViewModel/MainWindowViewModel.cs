using aemarcoCommons.WpfTools.WindowStuff;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace WpfToolsManualTests.ViewModel;

public partial class MainWindowViewModel : ObservableObject
{

    private readonly IWindowService _windowService;
    public MainWindowViewModel(
        IWindowService windowService)
    {
        _windowService = windowService;
    }

    [RelayCommand]
    private void Submit()
    {
        MessageBox.Show("Clicked");
    }


    [RelayCommand]
    private void Window()
    {
        _windowService.Show<TestViewModel>();
    }

    [RelayCommand]
    private void Dialog()
    {
        var question = new TestManualDialogViewModel("What is the answer?");
        var dialogResult = _windowService.ShowDialog(question);
        if (dialogResult is true)
        {
            MessageBox.Show($"Answer is {question.Answer}");
        }
    }

}