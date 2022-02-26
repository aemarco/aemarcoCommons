using System;
using System.Diagnostics;
using System.Text;

namespace aemarcoCommons.Toolbox.ShellTools
{
    public static class PowerShellHelper
    {
        public static PowerShellCommandResult Execute(string command, bool throwExceptions = true)
        {

            var base64 = Convert.ToBase64String(Encoding.Unicode.GetBytes(command));
            var process = new Process
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy unrestricted -EncodedCommand {base64}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var result = new PowerShellCommandResult
            {
                Command = command,
                Output = process.StandardOutput.ReadToEnd(),
                Errors = process.StandardError.ReadToEnd(),
                ExitCode = process.ExitCode
            };

            if (throwExceptions && result.ExitCode != 0)
            {
                throw new Exception($"Command exited with Code {result.ExitCode}.{Environment.NewLine} Output: {result.Output}{Environment.NewLine} Errors: {result.Errors}");
            }

            return result;
        }

    }


    public class PowerShellCommandResult
    {
        public string Command { get; set; }
        public string Output { get; set; }
        public string Errors { get; set; }
        public int ExitCode { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("##########################################################");

            if (!string.IsNullOrWhiteSpace(Output))
            {
                sb.AppendLine("Output:");
                sb.AppendLine(Output);
            }

            if (!string.IsNullOrWhiteSpace(Errors) && ExitCode != 0)
            {
                sb.AppendLine("Errors:");
                sb.AppendLine(Errors);
            }

            sb.AppendLine(ExitCode == 0
                ? $"Success for command: {Command}"
                : $"Fail for command: {Command}, Exit code: {ExitCode}");

            sb.AppendLine("##########################################################");

            return sb.ToString();
        }

        public void ThrowIfNoSuccess()
        {
            if (ExitCode != 0) throw new Exception(Errors);
        }


    }
}
