using System;
using System.Diagnostics;
using System.IO;
// ReSharper disable All

namespace aemarcoCommons.Toolbox.CommandTools
{
    [Obsolete("Use aemarcoCommons.Toolbox.SecurityTools.VirusScanService instead. ATTENTION new method does not delete unsave files")]
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


    [Obsolete("Use aemarcoCommons.Toolbox.SecurityTools.VirusScanResult instead.")]
    public class VirusScanResult
    {

        private VirusScanResult(string filePath, bool success)
        {
            FilePath = filePath;
            Success = success;
        }

        public VirusScanResult(string filePath, bool isSafe, string message)
            : this(filePath, true)
        {
            IsSafe = isSafe;
            Message = message;
        }


        public VirusScanResult(string filePath, Exception exception)
            : this(filePath, false)
        {
            Exception = exception;
        }



        public string FilePath { get; }
        public bool Success { get; }
        public bool IsSafe { get; }
        public string Message { get; }
        public Exception Exception { get; }
    }


}
