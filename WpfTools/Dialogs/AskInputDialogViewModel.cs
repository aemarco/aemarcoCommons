using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace aemarcoCommons.WpfTools.Dialogs;
public partial class AskInputDialogViewModel : DialogViewModel
{

    public override string Title => "Question";

    public AskInputDialogViewModel(string question)
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
