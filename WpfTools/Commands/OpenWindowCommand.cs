using aemarcoCommons.WpfTools.BaseModels;
using Autofac;

namespace aemarcoCommons.WpfTools.Commands
{
    public class OpenWindowCommand<T> : DelegateCommand where T : IWindow
    {
        public OpenWindowCommand()
        {
            CommandAction = _ =>
            {
                var wind = BootstrapperExtensions.RootScope.Resolve<T>();
                wind.Show();
            };
        }
    }
}
