namespace aemarcoCommons.ToolboxAppOptions.Services;

internal class StartupValidationService : BackgroundService
{

    private readonly IServiceProvider _serviceProvider;
    private readonly ToolboxAppOptionsSettings _configurationOptions;
    public StartupValidationService(
        IServiceProvider serviceProvider,
        ToolboxAppOptionsSettings configurationOptions)
    {
        _serviceProvider = serviceProvider;
        _configurationOptions = configurationOptions;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        List<Exception>? exceptions = null;

        foreach (var type in _configurationOptions.ConfigurationTypes)
        {
            try
            {
                _ = _serviceProvider.GetRequiredService(type);
            }
            catch (OptionsValidationException ex)
            {
                exceptions ??= [];
                exceptions.Add(ex);
            }
        }

        if (exceptions is not { Count: > 0 })
            return Task.CompletedTask;

        if (exceptions.Count == 1)
        {
            // Rethrow if it's a single error
            ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
        }

        throw new AggregateException(exceptions);

    }
}
