using System;
// ReSharper disable All

namespace aemarcoCommons.Toolbox.ConsoleTools
{

    [Obsolete("Use aemarcoCommons.ConsoleTools.PowerConsole instead.")]
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
