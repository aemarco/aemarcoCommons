using System;
using System.Threading.Tasks;
using System.Windows;
#nullable enable

namespace aemarcoCommons.WpfTools.Commands
{
    public class ExitApplicationCommand : AsyncBaseCommand
    {
        private readonly Func<Task>? _beforeClose;

        public ExitApplicationCommand(Func<Task>? beforeClose = null)
        {
            _beforeClose = beforeClose;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            if (_beforeClose is not null)
                await _beforeClose();
            Application.Current.Shutdown();
        }
    }
}
