namespace aemarcoCommons.ToolboxVlc.Services;

public class VlcPlayer
{

    private readonly VlcOptions _options;
    private readonly ILogger<VlcPlayer> _logger;
    public VlcPlayer(
        VlcOptions options,
        ILogger<VlcPlayer> logger)
    {
        _options = options;
        _logger = logger;
    }


    public CommandTask<CommandResult> PlayVideo(
        string videoPath,
        Action<VlcPlayOptionsBuilder>? options = null,
        CancellationToken cancellationToken = default)
    {
        var vlcPath = VlcPlayerExtensions.GetVlcExePath();
        if (vlcPath is null)
            throw new InvalidOperationException("VLC is not installed");

        var builder = new VlcPlayOptionsBuilder();
        options?.Invoke(builder);
        var config = builder.Build();

        _logger.LogInformation("Playing Video {videoPath}", videoPath);
        var cmd = Cli.Wrap(vlcPath)
            .WithArguments(args =>
            {
                if (config.Fullscreen)
                    args.Add("--fullscreen");
                if (config.PlayAndExit)
                    args.Add("--play-and-exit");

                if (!string.IsNullOrWhiteSpace(config.Title))
                    args.Add($"--meta-title={config.Title}");
                if (!string.IsNullOrWhiteSpace(config.Url))
                    args.Add($"--meta-url={config.Url}");
                if (!string.IsNullOrWhiteSpace(config.Genre))
                    args.Add($"--meta-genre={config.Genre}");

                args
                    //always include http interface
                    .Add("--extraintf").Add("http")
                    .Add("--http-password").Add(_options.HttpPassword)
                    //video to play
                    .Add(videoPath);
            });
        return cmd.ExecuteAsync(cancellationToken);
    }














}