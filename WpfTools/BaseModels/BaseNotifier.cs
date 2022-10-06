using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace aemarcoCommons.WpfTools.BaseModels
{
    public class BaseNotifier : IBaseNotifier
    {

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null || PropertyChanged == null) return;

            foreach (var d in PropertyChanged.GetInvocationList())
            {
                if (!(d.Target is ISynchronizeInvoke sync))
                {
                    d.DynamicInvoke(this, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    sync.BeginInvoke(d, new object[] { this, new PropertyChangedEventArgs(propertyName) });
                }
            }
        }


        protected virtual void NotifyPropertyChanged(
            [CallerMemberName] string propertyName = null) => OnPropertyChanged(propertyName);

        #endregion


    }
}
