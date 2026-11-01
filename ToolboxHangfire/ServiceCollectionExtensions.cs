using aemarcoCommons.ToolboxHangfire.Configuration;
using aemarcoCommons.ToolboxHangfire.Services;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace aemarcoCommons.ToolboxHangfire;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Wraps AddHangfire, applies our defaults and registers the tools of this project.
        /// Defaults are <see cref="LogEverythingFilter"/>, data compatibility level 1.8,
        /// simple assembly name type serializer and the recommended serializer settings.
        /// </summary>
        /// <param name="configureHangfire">Applied after the defaults, for everything specific like storage or dashboard pages</param>
        /// <param name="configureTools">Configures the tools of this project, see <see cref="HangfireToolsOptions"/></param>
        public IServiceCollection SetupHangfireWithTools(
            Action<IServiceProvider, IGlobalConfiguration> configureHangfire,
            Action<HangfireToolsOptions>? configureTools = null)
        {
            SetupHangfireTools(services, configureTools);
            SetupHangfire(services, configureHangfire);
            return services;
        }
    }
    private static void SetupHangfireTools(
        IServiceCollection services,
        Action<HangfireToolsOptions>? configure)
    {
        var options = new HangfireToolsOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        services.AddTransient<NoOp>();

        if (options.LogJobLifecycle)
            services.AddSingleton<LogEverythingFilter>();

        if (options.UpdateRecurringSchedule)
            services.AddHostedService<UpdateHangfireRecurringSchedule>();
    }


    private static void SetupHangfire(
        IServiceCollection services,
        Action<IServiceProvider, IGlobalConfiguration> configure)
    {
        services.AddHangfire((sp, config) =>
        {
            //registered by the tools, unless left away
            if (sp.GetService<LogEverythingFilter>() is { } logFilter)
                config.UseFilter(logFilter);

            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings();

            configure(sp, config);
        });
    }


}
