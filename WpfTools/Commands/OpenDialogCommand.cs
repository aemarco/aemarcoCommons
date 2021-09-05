using aemarcoCommons.WpfTools.BaseModels;
using Autofac;

namespace aemarcoCommons.WpfTools.Commands
{
    public class OpenDialogCommand<T> : DelegateCommand where T : IWindow
    {
        public OpenDialogCommand()
        {
            CommandAction = () =>
            {
                var wind = Bootstrapper.RootScope.Resolve<T>();
                wind.ShowDialog();
            };
        }
    }
}