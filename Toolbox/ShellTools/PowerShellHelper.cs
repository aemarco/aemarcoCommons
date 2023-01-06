using CliWrap;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.ShellTools
{
    public static class PowerShellHelper
    {
        public static async Task<CommandResult> RunCommand(
            string command,
            Action<string> output = null,
            params string[] args)
        {
            var cmd = CreateCommand(command, args);

            if (output != null)
            {
                cmd = cmd.WithStandardOutputPipe(PipeTarget.ToDelegate(output));
            }

            var result = await cmd.ExecuteAsync();
            return result;
        }

        public static Command CreateCommand(string command, params string[] args)
        {
            var result = Cli.Wrap("powershell.exe")
                .WithArguments(b =>
                {
                    b.Add("-NoProfile");
                    b.Add("-ExecutionPolicy").Add("unrestricted");
                    b.Add(command);
                    foreach (var arg in args)
                    {
                        b.Add(arg);
                    }
                });

            return result;
        }



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
