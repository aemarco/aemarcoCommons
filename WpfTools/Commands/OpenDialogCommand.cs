using aemarcoCommons.WpfTools.BaseModels;
using System.Windows;

namespace aemarcoCommons.WpfTools.Commands
{
    public class OpenDialogCommand<T> : DelegateCommand where T : Window
    {
        public OpenDialogCommand(T window)
        {
            CommandAction = () => window.ShowDialog();
        }
    }
}