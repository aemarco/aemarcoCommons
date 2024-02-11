namespace ToolboxTopServiceManualTests;

internal static class SampleServiceExtensions
{
    internal static HostApplicationBuilder SetupSampleService(
        this HostApplicationBuilder app)
    {
        app.Services.AddHostedService<SampleService>();
        return app;
    }
}

internal class SampleService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
            await Console.Out.WriteLineAsync("Hello from SampleService");
        }
    }
}
