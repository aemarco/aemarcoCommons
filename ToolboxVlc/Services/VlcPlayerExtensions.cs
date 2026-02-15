using Microsoft.Win32;

namespace aemarcoCommons.ToolboxVlc.Services;

public static class VlcPlayerExtensions
{

    public static bool IsInstalled
    {
        get
        {
            var folder = GetVlcFolder();
            return folder is not null;
        }
    }

    public static string? GetVlcExePath()
    {
        var folder = GetVlcFolder();
        if (folder is null)
            return null;
        var result = Path.Combine(folder, "vlc.exe");
        return File.Exists(result) ? result : null;
    }

    public static string? GetVlcFolder()
    {
        using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\VideoLAN\VLC");
        var result = key?.GetValue("InstallDir") as string;
        return result;
    }


}
