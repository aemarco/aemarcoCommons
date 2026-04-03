using CommunityToolkit.Mvvm.ComponentModel;

namespace aemarcoCommons.WpfTools.Dialogs;

public class DialogViewModel : ObservableObject
{

    public virtual string Title => "Title";

    public event EventHandler<bool?>? CloseDialog;
    protected virtual void OnCloseDialog(bool? dialogResult) =>
        CloseDialog?.Invoke(this, dialogResult);

}