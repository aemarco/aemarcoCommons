

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public abstract class ConsoleProgressBar
{
    public abstract void UpdateProgress(long done, long total);

    private ConsoleColor _background = Console.BackgroundColor;
    private ConsoleColor _foreground = Console.ForegroundColor;
    protected void RememberColors()
    {
        _background = Console.BackgroundColor;
        _foreground = Console.ForegroundColor;
    }
    protected void RestoreColors()
    {
        Console.BackgroundColor = _background;
        Console.ForegroundColor = _foreground;
    }

    protected void DrawProgressLine(long done, long total)
    {

        try
        {
            //draw empty progress bar
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write("["); //start
            var endPos = Console.WindowWidth - 10; //10 for write %ages
            Console.CursorLeft = endPos;
            Console.Write("]"); //end


            //draw filled part
            Console.CursorLeft = 1;
            var position = 1;

            if (total > 0)
            {
                var oneChunk = (1.0 * endPos - 2) / total;
                for (var i = 0; i < oneChunk * done; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.CursorLeft = position++;
                    Console.Write(" ");
                }
            }

            //because rounding in chunks does not reach 100%
            if (done >= total)
            {
                while (position < endPos)
                {
                    Console.CursorLeft = position++;
                    Console.Write(" ");
                }
            }


            //draw unfilled part
            Console.BackgroundColor = _background;
            while (position < endPos)
            {
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = endPos + 1;
            var progressString = done > 0
                ? $"{(int)(done * 100 / total)}".PadLeft(3)
                : "0".PadLeft(3);
            Console.WriteLine($"  {progressString} %");

        }
        catch
        {
            Console.WriteLine("ERROR UPDATING PROGRESS");
        }
    }

}

/// <summary>
/// This progress bar fits, if no console outputs are written in between progress steps
/// </summary>
public class ConsoleOneLineProgressBar : ConsoleProgressBar
{
    public override void UpdateProgress(long done, long total)
    {
        Console.CursorLeft = 0;
        RememberColors();
        //draw progress bar
        DrawProgressLine(done, total);
        if (done < total) Console.CursorTop--;
        RestoreColors();
    }
}