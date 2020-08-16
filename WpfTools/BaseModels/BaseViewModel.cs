using System;
using System.Windows.Input;
using WpfTools.Commands;

namespace WpfTools.BaseModels
{
    public class BaseViewModel : BaseNotifier, IBaseViewModel
    {
        public virtual DelegateCommand CloseCommand { get; } = new DelegateCommand();

    }
}
