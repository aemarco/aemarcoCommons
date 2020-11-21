using System;
using System.Collections.Generic;
using System.Text;

namespace aemarcoCommons.Toolbox.ConsoleTools
{
    public static class ConsoleCommands
    {
        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }


    }
}
