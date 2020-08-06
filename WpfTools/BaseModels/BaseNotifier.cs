using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfTools.BaseModels
{
    public class BaseNotifier : IBaseNotifier
    {
        protected bool NotifyPropertyChangedSync { get; set; } = true; 

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (propertyName == null || PropertyChanged == null) return;

            if (NotifyPropertyChangedSync)
                foreach (var d in PropertyChanged.GetInvocationList())
                {
                    if (!(d.Target is ISynchronizeInvoke sync))
                    {
                        d.DynamicInvoke(this, new PropertyChangedEventArgs(propertyName));
                    }
                    else
                    {
                        sync.BeginInvoke(d, new object[] {this, new PropertyChangedEventArgs(propertyName)});
                    }
                }
            else
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       
        #endregion


    }
}
