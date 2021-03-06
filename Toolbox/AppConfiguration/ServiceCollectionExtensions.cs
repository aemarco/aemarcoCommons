﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace aemarcoCommons.Toolbox.AppConfiguration
{
    public static class ServiceCollectionExtensions
    {

        public static IConfigurationRoot AddConfigurationUtils(this ServiceCollection serviceCollection,
            IConfigurationBuilder configBuilder,
            Action<ConfigurationOptions> options = null)
        {
            var toolConfig = new ConfigurationOptions();
            options?.Invoke(toolConfig);

            //register options
            //builder.RegisterInstance(toolConfig)
            //    .AsSelf()
            //    .SingleInstance();
            serviceCollection.AddSingleton(toolConfig);


            //register all settings classes
            //builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
            //    .Where(t => t.IsSubclassOf(typeof(SettingsBase)))
            //    .AsSelf()
            //    .AsImplementedInterfaces()
            //    .SingleInstance();
            foreach (var type in AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsSubclassOf(typeof(SettingsBase))))
            {
                serviceCollection.AddSingleton(type);
                foreach (var contract in type.GetInterfaces())
                {
                    serviceCollection.AddSingleton(contract, x => x.GetRequiredService(type));
                }
            }

            //register saved stuff which overrides default values
            if (!string.IsNullOrWhiteSpace(toolConfig.SettingsSaveDirectory))
            {
                foreach (var type in AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.IsSubclassOf(typeof(SettingsBase))))
                {
                    var filePath = type.GetSavePathForSetting(toolConfig);
                    //those files get registered even if not exist (may avoid problems with reloading :)
                    configBuilder.AddJsonFile(filePath, true, true);
                }
            }

            //register IConfiguration stuff
            var rootConfig = configBuilder.Build();


            //builder.RegisterInstance(rootConfig)
            //    .As<IConfiguration>()
            //    .As<IConfigurationRoot>()
            //    .SingleInstance();
            serviceCollection
                .AddSingleton(rootConfig)
                .AddSingleton(x => x.GetRequiredService<IConfiguration>());




            //builder.RegisterBuildCallback(scope =>
            //{
            //    SettingsBase.ConfigurationRoot = scope.Resolve<IConfigurationRoot>();
            //    SettingsBase.ConfigurationOptions = scope.Resolve<ConfigurationOptions>();
            //});
            SettingsBase.ConfigurationRoot = rootConfig;
            SettingsBase.ConfigurationOptions = toolConfig;


            return rootConfig;
        }










    }
}
