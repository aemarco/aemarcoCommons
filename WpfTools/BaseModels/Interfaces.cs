using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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
