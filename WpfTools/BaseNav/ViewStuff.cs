using aemarcoCommons.WpfTools.BaseModels;
using aemarcoCommons.WpfTools.Commands;

namespace aemarcoCommons.WpfTools.BaseNav
{

    //user controls
    public interface INavView //inherit in specific interface for view
    {
        object DataContext { get; set; }

    }

    //view models
    public interface INavViewModel : IBaseViewModel //inherit in specific interface for view model, inherit from BaseNaveViewModel
    {
        BaseNavWindowViewModel WindowViewModel { get; set; }
        INavView View { get; set; }
        string Title { get; }
    }




    public abstract class BaseNavViewModel : BaseViewModel, INavViewModel
    {
        protected BaseNavViewModel(INavView view)
        {
            View = view;
            View.DataContext = this;
        }


        public INavView View { get; set; }


        public BaseNavWindowViewModel WindowViewModel { get; set; }

        public virtual string Title => null;


        public override DelegateCommand CloseCommand =>
            new()
            {
                CommandAction = () =>
                {
                    WindowViewModel?.Window?.Close();
                }
            };
    }
}