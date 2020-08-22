using WpfTools.BaseModels;
using WpfTools.Commands;

namespace WpfTools.BaseNav
{
    public interface INavView //inherit in specific interface for view
    {
        object DataContext { get; set; }

    }
    public interface INavViewModel : IBaseViewModel //inherit in specific interface for view model, inherit from BaseNaveViewModel
    {
        INavWindowViewModel Window { get; set; }
        INavView View { get; set; }
       
    }

    public abstract class BaseNavViewModel : BaseViewModel, INavViewModel
    {
        protected BaseNavViewModel(INavView view)
        {
            View = view;
            View.DataContext = this;
        }

        public INavWindowViewModel Window { get; set; }
        public INavView View { get; set; }


        public override DelegateCommand CloseCommand
        {
            get
            {
                return new DelegateCommand()
                {
                   CommandAction = () =>
                   {
                       Window.Window.Close();
                   }
                };
            }
        }
    }
}