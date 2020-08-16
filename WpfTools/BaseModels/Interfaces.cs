using System;
using System.ComponentModel;
using System.Windows.Input;
using WpfTools.Commands;
using WpfTools.Helpers;

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
