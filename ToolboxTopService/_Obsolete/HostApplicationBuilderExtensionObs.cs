using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Topshelf;
using Topshelf.HostConfigurators;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ToolboxTopService;

//TODO: remove topshelf nuget after removing this

public static class HostApplicationBuilderExtensionObs
{
    [Obsolete("Use RunAsTopService instead.")]
    public static TopshelfExitCode RunAsTopshelfService(
        this HostApplicationBuilder app,
        Action<HostConfigurator> configureCallback,
        Action<IHost>? beforeHostStarting = null,
        Action<IHost>? afterHostStopped = null)
    {
        if (configureCallback == null)
            throw new ArgumentNullException(nameof(configureCallback));

        app.Services.AddSingleton<IHostLifetime, TopshelfLifetime>();

        IHost? host = null;
        try
        {
            var rc = HostFactory.Run(x =>
            {
                configureCallback(x);
                x.Service<IHost>(s =>
                {
                    s.ConstructUsing(() =>
                    {
                        host = app.Build();
                        return host;
                    });
                    s.WhenStarted(service =>
                    {
                        beforeHostStarting?.Invoke(service);
                        service.Start();
                    });
                    s.WhenStopped(service =>
                    {
                        service.StopAsync().Wait();
                        afterHostStopped?.Invoke(service);
                    });
                });
            });
            return rc;
        }
        finally
        {
            host?.Dispose();
        }
    }
}

public class TopshelfLifetime : IHostLifetime
{
    public TopshelfLifetime(
        IHostApplicationLifetime applicationLifetime)
    {
        ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
    }

#pragma warning disable IDE0052
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private IHostApplicationLifetime ApplicationLifetime { get; }
#pragma warning restore IDE0052

    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
