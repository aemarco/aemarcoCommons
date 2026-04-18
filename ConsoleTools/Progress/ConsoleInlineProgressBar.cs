// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

/// <summary>
/// This progress bar fits, if console outputs are written in between progress steps
/// </summary>
[Obsolete("Use Spectre.Console progress via PowerConsole.StartProgress in aemarcoCommons.ToolboxConsole instead.")]
public class ConsoleInlineProgressBar : ConsoleProgressBar
{
    public override void UpdateProgress(long done, long total)
    {
        if (Console.CursorLeft > 0) Console.WriteLine();
        RememberColors();
        //draw progress bar
        DrawProgressLine(done, total);
        RestoreColors();
    }
}