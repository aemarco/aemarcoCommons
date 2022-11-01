using aemarcoCommons.WpfTools.Commands;
using System.ComponentModel;

namespace aemarcoCommons.WpfTools.BaseModels;

public interface IBaseNotifier : INotifyPropertyChanged { }

public interface IBaseViewModel : IBaseNotifier
{
    DelegateCommand CloseCommand { get; }
}

public interface IWindow { }