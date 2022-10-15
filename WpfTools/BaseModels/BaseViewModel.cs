using aemarcoCommons.WpfTools.Commands;

namespace aemarcoCommons.WpfTools.BaseModels;

public class BaseViewModel : BaseNotifier, IBaseViewModel
{
    public virtual DelegateCommand CloseCommand { get; } = new DelegateCommand();

}