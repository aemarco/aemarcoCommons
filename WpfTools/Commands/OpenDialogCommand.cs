using WpfTools.BaseModels;

namespace WpfTools.Commands
{
    public class OpenDialogCommand<T> : DelegateCommand where T : IWindow
    {
        public OpenDialogCommand(T window)
        {
            CommandAction = () => window.ShowDialog();
        }
    }
}