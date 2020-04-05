using System;
using System.Diagnostics;
using System.IO;
using Contracts.Toolbox;

namespace Toolbox.CommandTools
{
    public static class SecurityTools
    {
        public static VirusScanResult ScanFile(string filePath)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = @"C:\Program Files\Windows Defender\MpCmdRun.exe",
                    Arguments = $"-Scan -ScanType 3 -File {filePath}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var process = Process.Start(startInfo);
                using (var streamReader = process?.StandardOutput)
                {
                    var message = streamReader?.ReadToEnd();
                    var isSave = process?.ExitCode == 0 &&
                             !string.IsNullOrEmpty(message) &&
                             message.Contains("found no threats");
                    if (!isSave && File.Exists(filePath)) File.Delete(filePath);
                    return new VirusScanResult(filePath, isSave, message);
                }
            }
            catch (Exception ex)
            {
                return new VirusScanResult(filePath, ex);
            }
        }















    }
}
