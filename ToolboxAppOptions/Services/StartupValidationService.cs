namespace aemarcoCommons.ToolboxAppOptions.Services;

internal class StartupValidationService : IHostedService
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


    public Task StartAsync(CancellationToken cancellationToken)
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
            ExceptionDispatchInfo.Capture(exceptions[0]).Throw();

        throw new AggregateException(exceptions);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
