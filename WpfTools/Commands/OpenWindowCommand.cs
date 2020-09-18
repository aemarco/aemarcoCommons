using aemarcoCommons.WpfTools.BaseModels;

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
