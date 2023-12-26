using CliWrap;
using System.Threading.Tasks;

namespace aemarcoCommons.ConsoleTools.Interop;

public static class PowerShellHelper
{
    public static async Task RunCommand(
        string command,
        params string[] args)
    {
        try
        {
            var cmd = Toolbox.ShellTools.PowerShellHelper.CreateCommand(command, args)
                .WithStandardOutputPipe(PipeTarget.ToDelegate(x => AnsiConsole.MarkupLine("[grey50]{0}[/]", x.EscapeMarkup())));
            AnsiConsole.MarkupLine("[green]{0}[/]", $"Starting {cmd}".EscapeMarkup());
            var result = await cmd.ExecuteAsync();

            AnsiConsole.MarkupLine($"[green]Success[/] ({result.RunTime.TotalSeconds:N2} sec.)");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

}
