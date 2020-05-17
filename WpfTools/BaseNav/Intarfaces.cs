using WpfTools.BaseModels;

namespace WpfTools.BaseNav
{
    //windows
    public interface INavMainWindow : INavWindow { }
    public interface INavWindow
    {
        object DataContext { get; set; }
    }

    public interface INavMainWindowViewModel : INavWindowViewModel { }
    public interface INavWindowViewModel : IBaseViewModel
    {
        INavWindow Window { get; set; }
        INavView View { get; set; }
        void ShowViewFor<T>() where T : INavViewModel;
    }
    //view
    public interface INavView
    {
        object DataContext { get; set; }

    }
    public interface INavViewModel : IBaseViewModel
    {
        INavWindowViewModel Window { get; set; }
        INavView View { get; set; }
       
    }
}
