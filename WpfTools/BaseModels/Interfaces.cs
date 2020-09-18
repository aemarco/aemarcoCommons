using System;
using System.ComponentModel;
using aemarcoCommons.WpfTools.Commands;

namespace aemarcoCommons.WpfTools.BaseModels
{
    public interface IBaseNotifier : INotifyPropertyChanged { }

    public interface IBaseViewModel : IBaseNotifier
    {
        DelegateCommand CloseCommand { get; }
    }
    public interface IBaseService : IBaseNotifier { }

    public interface IWindow
    {
        void Show();
        bool? ShowDialog();

        [Obsolete]
        void Close();

    }



}
