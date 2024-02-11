using System.Text;

namespace aemarcoCommons.ToolboxTopService;

//https://github.com/Tyrrrz/CliWrap
//https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/sc-config

internal record TopService(
    string DllPath,
    string ServiceName,
    string DisplayName,
    string Description,
    StartupType StartupType)
{

    internal async Task Install()
    {

        var output = new StringBuilder();
        var error = new StringBuilder();

        //install
        var cmd =
            Cli.Wrap("sc.exe")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(output))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(error))
                .WithArguments(x => x
                    .Add("create").Add(ServiceName)
                    .Add("binpath=").Add($"{DllPath[..^3]}exe")
                    .Add("displayname=").Add(DisplayName));
        await HandleCommand(cmd, skipOutput: true);

        //description
        if (Environment.ExitCode == 0 && !string.IsNullOrWhiteSpace(Description))
        {
            var descriptionCmd = Cli.Wrap("sc.exe")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(output))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(error))
                .WithArguments(x => x
                    .Add("description").Add(ServiceName).Add(Description));
            await HandleCommand(descriptionCmd, skipOutput: true);
        }

        //startup type
        if (Environment.ExitCode == 0 && StartupType != StartupType.Manual)
        {
            var mode = this.StartupType switch
            {
                StartupType.Manual => "demand",
                StartupType.Auto => "auto",
                StartupType.AutoDelayed => "delayed-auto",
                StartupType.Disabled => "disabled",
                _ => throw new NotImplementedException()
            };

            var startupCmd = Cli.Wrap("sc.exe")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(output))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(error))
                .WithArguments(x => x
                    .Add("config").Add(ServiceName)
                    .Add("start=").Add(mode));
            await HandleCommand(startupCmd, skipOutput: true);
        }


        //output
        if (output.ToString() is { Length: > 0 } outputMessage)
        {
            Console.WriteLine(outputMessage);
        }
        if (error.ToString() is { Length: > 0 } errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }

        //message
        Console.ForegroundColor = ConsoleColor.Cyan;
        var message = Environment.ExitCode == 0
            ? "Successfully installed"
            : "Failed installing";
        Console.WriteLine(message);
        Console.ResetColor();
    }
    internal async Task UnInstall()
    {
        var cmd = Cli.Wrap("sc.exe")
            .WithArguments(x => x
                .Add("delete").Add(ServiceName));
        await HandleCommand(
            cmd,
            "Successfully uninstalled",
            "Failed uninstalling");
    }
    internal async Task Start()
    {
        var cmd = Cli.Wrap("sc.exe")
            .WithArguments(x => x
                .Add("start").Add(ServiceName));
        await HandleCommand(
            cmd,
            "Successfully started",
            "Failed starting");
    }
    internal async Task Stop()
    {
        var cmd = Cli.Wrap("sc.exe")
            .WithArguments(x => x
                .Add("stop").Add(ServiceName));
        await HandleCommand(
            cmd,
            "Successfully stopped",
            "Failed stopping");
    }


    private async Task HandleCommand(
        Command command,
        string? successMessage = null,
        string? failMessage = null,
        bool skipOutput = false)
    {

        var result = await command
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();

        if (!skipOutput)
        {
            if (!string.IsNullOrWhiteSpace(result.StandardOutput))
                Console.WriteLine(result.StandardOutput);
            if (!string.IsNullOrWhiteSpace(result.StandardError))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.StandardError);
                Console.ResetColor();
            }
        }

        var message = result.ExitCode == 0
            ? successMessage
            : failMessage;
        if (!string.IsNullOrWhiteSpace(message))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        Environment.ExitCode = result.ExitCode;
    }

}