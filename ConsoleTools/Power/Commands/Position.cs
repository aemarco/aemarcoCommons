﻿using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public static partial class PowerConsole
{
    public static void ClearCurrentLine()
    {
        var currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }

}