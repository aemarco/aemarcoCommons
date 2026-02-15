// ReSharper disable NotAccessedPositionalProperty.Global
namespace aemarcoCommons.ToolboxVlc.Services;

public class VlcPlayer : IDisposable
{

    private readonly VlcOptions _options;
    private readonly IVlcClient _vlcClient;
    private readonly ILogger<VlcPlayer> _logger;
    private CancellationTokenSource? _cts;
    private CommandTask<CommandResult>? _playerTask;
    public VlcPlayer(
        VlcOptions options,
        IVlcClient vlcClient,
        ILogger<VlcPlayer> logger)
    {
        _options = options;
        _vlcClient = vlcClient;
        _logger = logger;
    }

    public bool IsRunning => _playerTask is { Task.IsCompleted: false };

    public PlaybackContext Play(
        string videoPath,
        Action<VlcPlayOptionsBuilder>? options = null)
    {
        Stop();

        var vlcPath = VlcPlayerExtensions.GetVlcExePath();
        if (vlcPath is null)
            throw new InvalidOperationException("VLC is not installed");

        var builder = new VlcPlayOptionsBuilder();
        options?.Invoke(builder);
        var config = builder.Build();

        _cts = new CancellationTokenSource();
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
        _playerTask = cmd.ExecuteAsync(_cts.Token);
        return new PlaybackContext(_playerTask.Task, _cts.Token);
    }

    public async Task<bool> IsVideoPlaying(CancellationToken cancellationToken = default)
    {
        if (!IsRunning)
            return false;

        try
        {
            var status = await _vlcClient.GetStatus(cancellationToken);
            return status is { State: "playing", Time: > 0 };
        }
        catch (Exception ex)
        {
            //if vlc http is not ready yet, it can throw
            _logger.LogWarning(ex, "Could not get vlc status");
            return false;
        }
    }

    public void Stop()
    {
        if (_cts is { IsCancellationRequested: false })
        {
            _cts.Cancel();
        }
    }

    public void Dispose()
    {
        Stop();
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }

}

public record PlaybackContext(Task Task, CancellationToken CancellationToken);