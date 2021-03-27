using System;
using System.Diagnostics;
using System.IO;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global


namespace aemarcoCommons.Toolbox.SecurityTools
{
    public static class VirusScanService
    {
        public static VirusScanResult ScanFile(
            string filePath,
            string mpCmdRunExeLocation = @"C:\Program Files\Windows Defender\MpCmdRun.exe",
            int timeoutInMs = 30000)
        {
            //ensure files are present
            if (!File.Exists(mpCmdRunExeLocation))
                return new VirusScanResult(new FileNotFoundException("Could not found Virus scanner exe", mpCmdRunExeLocation));
            if (!File.Exists(filePath))
                return new VirusScanResult(new FileNotFoundException("Could not found file to scan", filePath));

            var fileInfo = new FileInfo(filePath);
            var process = new Process();
            var startInfo = new ProcessStartInfo(mpCmdRunExeLocation)
            {
                Arguments = $"-Scan -ScanType 3 -File \"{fileInfo.FullName}\" -DisableRemediation",
                CreateNoWindow = true,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit(timeoutInMs);

            if (!process.HasExited)
            {
                process.Kill();
                return new VirusScanResult(new TimeoutException("Scan timed out"));
            }

            switch (process.ExitCode)
            {
                case 0:
                    return new VirusScanResult(false);
                case 2:
                    return new VirusScanResult(true);
                default:
                    return new VirusScanResult(new Exception("Ups, something went south."));
            }
        }
    }

    public readonly struct VirusScanResult
    {
        public VirusScanResult(bool isThread)
            : this(true, isThread, null)
        { }

        public VirusScanResult(Exception exception)
            : this(false, null, exception)
        { }

        private VirusScanResult(bool success, bool? isThread, Exception exception)
        {
            Success = success;
            IsThread = isThread;
            Exception = exception;
        }

        public bool Success { get; }
        public bool? IsThread { get; }
        public Exception Exception { get; }
    }
}
