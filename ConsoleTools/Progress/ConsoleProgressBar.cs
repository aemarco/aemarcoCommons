using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools
{
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


    /// <summary>
    /// This progress bar fits, if console outputs are written in between progress steps
    /// </summary>
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

}
