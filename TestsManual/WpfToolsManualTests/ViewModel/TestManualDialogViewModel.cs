using aemarcoCommons.WpfTools.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfToolsManualTests.ViewModel;

public partial class TestManualDialogViewModel : DialogViewModel
{

    public override string Title => "Question";

    public TestManualDialogViewModel(string question)
    {
        Question = question;
    }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Question { get; }

    [ObservableProperty]
    private string _answer = string.Empty;

    [RelayCommand]
    private void Okay()
    {
        OnCloseDialog(true);
    }

}
