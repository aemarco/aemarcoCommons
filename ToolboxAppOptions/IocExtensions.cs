﻿using aemarcoCommons.ToolboxAppOptions.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace aemarcoCommons.ToolboxAppOptions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigOptionsUtils(
            this IServiceCollection services,
            IConfigurationRoot config,
            Action<ConfigurationOptionsBuilder> options = null)
        {

            //register IConfiguration stuff
            services.AddSingleton(config);
            services.AddSingleton<IConfiguration>(sp => sp.GetRequiredService<IConfigurationRoot>());


            //register tool config
            var toolConfigBuilder = new ConfigurationOptionsBuilder();
            options?.Invoke(toolConfigBuilder);
            var toolConfig = toolConfigBuilder
                .AddConfigurationTypes(AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.IsSubclassOf(typeof(SettingsBase)))
                    .ToArray())
                .Build();
            services.AddSingleton(toolConfig);

            //register all the config types
            foreach (var type in toolConfig.ConfigurationTypes)
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
                            Array.Empty<object>());
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

            //so that all validators are registered
            services.AddValidatorsFromAssemblies(toolConfig.ConfigurationAssemblies, ServiceLifetime.Singleton);

            //validation during startup
            if (toolConfig.EnableValidationOnStartup)
                services.AddHostedService<StartupValidationService>();

            return services;
        }
    }

    public static class AutoFacExtensions
    {
        public static ContainerBuilder AddConfigOptionsUtils(
            this ContainerBuilder builder,
            IConfigurationRoot config,
            Action<ConfigurationOptionsBuilder> options = null)
        {
            builder.Populate(
                new ServiceCollection()
                    .AddConfigOptionsUtils(
                        config,
                        options));

            builder.RegisterBuildCallback(scope =>
            {
                if (scope.TryResolve<StartupValidationService>(out var startValidator))
                {
                    startValidator.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
                }
            });

            return builder;
        }
    }

}
