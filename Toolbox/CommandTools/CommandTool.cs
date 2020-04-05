using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Toolbox.CommandTools
{

    public static class CommandTool
    {

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public static void OpenFileOrFolder(string path)
        {
#pragma warning disable IDE0067 // Objekte verwerfen, bevor Bereich verloren geht
            _ = new Process
            {
                StartInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                }
            }.Start();
#pragma warning restore IDE0067 // Objekte verwerfen, bevor Bereich verloren geht
        }


    }
}
