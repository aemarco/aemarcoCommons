using WpfTools.BaseModels;

namespace WpfTools.BaseNav
{
    public abstract class BaseNavViewModel : BaseViewModel, INavViewModel
    {
        protected BaseNavViewModel(INavView view)
        {
            View = view;
            View.DataContext = this;
        }

        public INavWindowViewModel Window { get; set; }
        public INavView View { get; set; }
    }
}