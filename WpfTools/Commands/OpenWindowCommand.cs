using aemarcoCommons.WpfTools.BaseModels;
using System.Windows;

namespace aemarcoCommons.WpfTools.Commands
{
    public class OpenWindowCommand<T> : DelegateCommand where T : IWindow
    {
        public OpenWindowCommand(T window)
        {
            CommandAction = window.Show;
        }
    }
}
