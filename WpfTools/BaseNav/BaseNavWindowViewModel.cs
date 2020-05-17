using WpfTools.BaseModels;

namespace WpfTools.BaseNav
{
    public abstract class BaseNavWindowViewModel : BaseViewModel, INavWindowViewModel
    {
        protected BaseNavWindowViewModel(INavWindow window)
        {
            Window = window;
            Window.DataContext = this;
        }

        public INavWindow Window { get; set; }
        public INavView View { get; set; }

        public void ShowViewFor<T>() where T : INavViewModel
        {
            var vm = Resolve<T>() as INavViewModel;
            vm.Window = this;
            View = vm.View;
            NotifyPropertyChanged(nameof(View));
        }

        protected abstract T Resolve<T>() where T : INavViewModel;
    }
}