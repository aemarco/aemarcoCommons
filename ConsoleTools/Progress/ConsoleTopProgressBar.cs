// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

/// <summary>
/// This progress bar fits, if a lot of console outputs are written in between progress steps.
/// Place the number of max. lines of content in between steps into the constructor. defaults to 1.
/// </summary>
public class ConsoleTopProgressBar : ConsoleProgressBar
{
    private readonly int _contentHeight;
    public ConsoleTopProgressBar(int contentHeightInBetweenSteps = 1)
    {
        if (contentHeightInBetweenSteps < 0) throw new ArgumentException(nameof(contentHeightInBetweenSteps));
        _contentHeight = contentHeightInBetweenSteps;

        Console.Clear();
        Console.CursorTop = 1;
    }

    public override void UpdateProgress(long done, long total)
    {
        //remember
        RememberColors();
        var xPos = Console.CursorLeft;
        var yPos = Console.CursorTop;


        //draw progress bar
        Console.SetCursorPosition(0, 0);
        DrawProgressLine(done, total);

        //restore
        RestoreColors();
        Console.SetCursorPosition(xPos, yPos);

        if (OperatingSystem.IsWindows())
        {
            //move content up
            while (Console.CursorTop > Console.WindowHeight - 1 - _contentHeight)
            {
                Console.MoveBufferArea(0, 2, Console.WindowWidth, yPos, 0, 1);
                Console.CursorTop--;
            }
            Console.SetWindowPosition(0, 0);
        }
    }
}