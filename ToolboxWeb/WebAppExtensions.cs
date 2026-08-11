using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace aemarcoCommons.ToolboxWeb;

public static class WebAppExtensions
{
    extension(WebApplicationBuilder app)
    {
        public WebApplicationBuilder SetupSerilogging(
            Action<LoggerConfiguration>? configure = null,
            bool preserveStaticLogger = false,
            bool writeToProviders = false)
        {
            if (!writeToProviders)
                app.Logging.ClearProviders();

            app.Services.AddSerilog(
                configureLogger =>
                {
                    configureLogger
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                        .MinimumLevel.Override("System", LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .Enrich.WithThreadId()
                        .Enrich.WithMachineName()
                        .Enrich.WithEnvironmentUserName()
                        .Enrich.WithEnvironmentName()
                        .Enrich.WithProperty("app", app.Environment.ApplicationName)
                        .Enrich.WithProperty("EnvironmentApplicationName", app.Environment.ApplicationName)
                        .ReadFrom.Configuration(app.Configuration);

                    configure?.Invoke(configureLogger);
                },
                preserveStaticLogger,
                writeToProviders);

            return app;
        }
    }
}
