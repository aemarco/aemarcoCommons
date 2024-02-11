using System.ComponentModel;

namespace aemarcoCommons.WpfTools.BaseModels;

public interface IBaseNotifier : INotifyPropertyChanged;

public interface IBaseViewModel : IBaseNotifier;

public interface IWindow
{
    void Show();
    bool? ShowDialog();
}