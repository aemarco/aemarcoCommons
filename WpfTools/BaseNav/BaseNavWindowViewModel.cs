using WpfTools.BaseModels;

namespace WpfTools.BaseNav
{

    //windows
    public interface INavMainWindow : INavWindow { } //implement this in the main window
    public interface INavWindow //inherit this interface for other windows
    {
        object DataContext { get; set; }
        void Close();
    }

    public interface INavMainViewModel : INavWindowViewModel { } //implement this in the main view model, inherit BaseNavWindowViewModel
    public interface INavWindowViewModel : IBaseViewModel //inherit this for other window view models
    {
        INavWindow Window { get; set; }
        INavView View { get; set; }
        void ShowViewFor<T>() where T : INavViewModel;
    }



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