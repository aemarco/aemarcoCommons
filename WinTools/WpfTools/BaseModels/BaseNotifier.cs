using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinTools.WpfTools.BaseModels
{
    public class BaseNotifier : IBaseNotifier
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (propertyName != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


    }
}
