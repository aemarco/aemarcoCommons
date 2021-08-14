using System;
using System.Windows;
#nullable enable

namespace aemarcoCommons.WpfTools.Commands
{
    public class ExitApplicationCommand : BaseCommand
    {
        private readonly Action? _beforeClose;

        public ExitApplicationCommand(Action? beforeClose = null)
        {
            _beforeClose = beforeClose;
        }

        public override void Execute(object? parameter)
        {
            _beforeClose?.Invoke();
            Application.Current.Shutdown();
        }
    }
}
