using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace aemarcoCommons.ConsoleTools.Interop;

internal static class WindowVisibility
{
    private const int SwHide = 0;
    private const int SwShow = 5;

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


    [SupportedOSPlatform("windows")]
    internal static void Hide()
    {
        var handle = GetConsoleWindow();
        ShowWindow(handle, SwHide);
    }

    [SupportedOSPlatform("windows")]
    internal static void Show()
    {
        var handle = GetConsoleWindow();
        ShowWindow(handle, SwShow);
    }
}