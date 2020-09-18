using aemarcoCommons.WpfTools.BaseModels;

namespace aemarcoCommons.WpfTools.Commands
{
    public class OpenDialogCommand<T> : DelegateCommand where T : IWindow
    {
        public OpenDialogCommand(T window)
        {
            CommandAction = () => window.ShowDialog();
        }
    }
}