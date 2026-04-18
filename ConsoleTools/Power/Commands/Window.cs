using aemarcoCommons.ConsoleTools.Interop;
using System.Runtime.Versioning;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;

public static partial class PowerConsole
{

    [Obsolete("Use aemarcoCommons.ToolboxConsole.PowerConsole instead.")]
    [SupportedOSPlatform("windows")]
    public static void ShowWindow()
    {
        WindowVisibility.Show();
    }

    [Obsolete("Use aemarcoCommons.ToolboxConsole.PowerConsole instead.")]
    [SupportedOSPlatform("windows")]
    public static void HideWindow()
    {
        WindowVisibility.Hide();
    }
}