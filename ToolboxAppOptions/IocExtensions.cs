using aemarcoCommons.ToolboxAppOptions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace aemarcoCommons.ToolboxAppOptions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigOptionsUtils(
        this IServiceCollection services,
        IConfigurationRoot config,
        Action<ConfigurationOptionsBuilder>? options = null)
    {

        //register IConfiguration stuff
        services.AddSingleton(config);
        services.AddSingleton<IConfiguration>(sp => sp.GetRequiredService<IConfigurationRoot>());


        //register tool config
        var toolConfigBuilder = new ConfigurationOptionsBuilder();
        options?.Invoke(toolConfigBuilder);
        var toolConfig = toolConfigBuilder.Build();
        services.AddSingleton(toolConfig);

        //register all the config types
        foreach (var type in toolConfig.IocConfigurationTypes)
        {
            //setup options build pipeline
            services.ConfigureOptions(typeof(AppOptionFactory<>).MakeGenericType(type));

            //register type as self resolved through options
            Type optionsType = typeof(IOptions<>).MakeGenericType(type);
            services.AddSingleton(type, sp =>
            {
                try
                {
                    return optionsType.InvokeMember(
                        "Value",
                        BindingFlags.GetProperty,
                        null,
                        sp.GetRequiredService(optionsType),
                        []) ?? throw new NullReferenceException(nameof(optionsType));
                }
                catch (TargetInvocationException ex) when (ex.InnerException is OptionsValidationException)
                {
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                    throw;
                }
            });

            //register also it´s interfaces
            var interfaces = type.GetInterfaces();
            foreach (Type interfaceType in interfaces)
            {
                services.AddSingleton(interfaceType, s => s.GetRequiredService(type));
            }
        }

        //so that all validators are registered, but only those which are covered by our lib
        services.AddValidatorsFromAssemblies(
            toolConfig.ConfigurationAssemblies,
            ServiceLifetime.Transient,
            x => x.ValidatorType.BaseType?
                .GetGenericArguments()
                .FirstOrDefault()?
                .IsAssignableTo(typeof(ISettingsBase)) ?? false);

        //validation during startup
        if (toolConfig.EnableValidationOnStartup)
            services.AddHostedService<StartupValidationService>();

        services.AddTransient<AppOptionsTransformationService>();


        return services;
    }
}
