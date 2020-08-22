using System;
using System.ComponentModel;
using WpfTools.Commands;

namespace WpfTools.BaseModels
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
