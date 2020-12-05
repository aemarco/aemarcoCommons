using System.Diagnostics;
using System.Runtime.InteropServices;

namespace aemarcoCommons.Toolbox.CommandTools
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

        public static Process OpenFileOrFolder(string path)
        {
            var result = new Process
            {
                StartInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                }
            }; 
            result.Start();
            return result;
        }


    }
}
