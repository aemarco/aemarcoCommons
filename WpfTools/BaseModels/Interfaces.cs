using System.ComponentModel;

namespace WpfTools.BaseModels
{
    public interface IBaseNotifier : INotifyPropertyChanged { }
    public interface IBaseViewModel : IBaseNotifier { }
    public interface IBaseService : IBaseNotifier { }

    public interface IWindow
    {
        void Close();
    }
}
