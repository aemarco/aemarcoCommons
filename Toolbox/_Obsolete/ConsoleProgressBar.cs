using System;

// ReSharper disable All
namespace aemarcoCommons.Toolbox.ConsoleTools
{

    [Obsolete("Use aemarcoCommons.ConsoleTools.ConsoleProgressBar instead.")]
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

            while (true)
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
                    int position = 1;

                    if (total > 0)
                    {
                        double onechunk = (1.0 * endPos - 2) / total;
                        for (int i = 0; i < onechunk * done; i++)
                        {
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.CursorLeft = position++;
                            Console.Write(" ");
                        }
                    }
                    //draw unfilled part
                    Console.BackgroundColor = _background;
                    for (int i = position; i < endPos; i++)
                    {
                        Console.CursorLeft = i;
                        Console.Write(" ");
                    }


                    //draw totals
                    Console.CursorLeft = endPos + 1;

                    var progressString = done > 0
                        ? $"{(int)(done * 100 / total)}".PadLeft(3)
                        : "0".PadLeft(3);
                    Console.WriteLine($"  {progressString} %");
                    break;
                }
                catch
                {
                    Console.WriteLine("ERROR UPDATING PROGRESS");
                }
            }
        }

    }


    [Obsolete("Use aemarcoCommons.ConsoleTools.ConsoleTopProgressBar instead.")]
    public class ConsoleTopProgressBar : ConsoleProgressBar
    {
        public ConsoleTopProgressBar()
        {
            Console.CursorTop = 1;
        }


        private int _contentHeight = 1;
        public int ContentHeight
        {
            get { return _contentHeight; }
            set { _contentHeight = value; }
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

            //move content up
            while (Console.CursorTop > Console.WindowHeight - 1 - _contentHeight)
            {
                Console.MoveBufferArea(0, 2, Console.WindowWidth, yPos, 0, 1);
                Console.CursorTop--;
            }
            Console.SetWindowPosition(0, 0);
        }


    }

    [Obsolete("Use aemarcoCommons.ConsoleTools.ConsoleInlineProgressBar instead.")]
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

}
