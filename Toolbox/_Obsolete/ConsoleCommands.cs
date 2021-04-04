using System;
// ReSharper disable All

namespace aemarcoCommons.Toolbox.ConsoleTools
{

    public static class ConsoleCommands
    {
        [Obsolete("Use aemarcoCommons.ConsoleTools.PowerConsole instead.")]
        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
