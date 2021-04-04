using aemarcoCommons.ConsoleTools.Interop;
using System.Runtime.Versioning;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools
{
    public static partial class PowerConsole
    {

        [SupportedOSPlatform("windows")]
        public static void ShowWindow()
        {
            WindowVisibility.Show();
        }

        [SupportedOSPlatform("windows")]
        public static void HideWindow()
        {
            WindowVisibility.Hide();
        }
    }
}