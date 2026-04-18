using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxConsole;

public static partial class PowerConsole
{
    public static void ClearCurrentLine()
    {
        var currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
    }

    [SupportedOSPlatform("windows")]
    public static void ShowWindow()
    {
        var handle = GetConsoleWindow();
        ShowWindow(handle, SwShow);
    }


    [SupportedOSPlatform("windows")]
    public static void HideWindow()
    {
        var handle = GetConsoleWindow();
        ShowWindow(handle, SwHide);
    }

    [LibraryImport("kernel32.dll")]
    private static partial nint GetConsoleWindow();

    private const int SwShow = 5;
    private const int SwHide = 0;

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    // ReSharper disable once UnusedMethodReturnValue.Local
    private static partial bool ShowWindow(nint hWnd, int nCmdShow);
}
