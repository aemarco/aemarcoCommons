using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace aemarcoCommons.Toolbox.AppConfiguration
{
    public static class AutofacExtensions
    {
        /// <summary>
        /// Register all necessary stuff to the Builder, so Setting-Classes can be injected later
        /// </summary>
        /// <param name="builder">builder used for registrations</param>
        /// <param name="configBuilder">config builder used for registrations</param>
        /// <param name="options">optional configuration for certain features</param>
        /// <returns>given builder, so that it can be chained</returns>
        public static IConfigurationRoot AddConfigurationUtils(this ContainerBuilder builder,
            IConfigurationBuilder configBuilder,
            Action<ConfigurationOptions> options = null)
        {
            //set options
            var toolConfig = new ConfigurationOptions();
            options?.Invoke(toolConfig);

            //register options
            builder.RegisterInstance(toolConfig)
                .AsSelf()
                .SingleInstance();

            //register all settings classes
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => t.IsSubclassOf(typeof(SettingsBase)))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

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
            builder.RegisterInstance(rootConfig)
                .As<IConfiguration>()
                .As<IConfigurationRoot>()
                .SingleInstance();

            builder.RegisterBuildCallback(scope =>
            {
                SettingsBase.ConfigurationRoot = scope.Resolve<IConfigurationRoot>();
                SettingsBase.ConfigurationOptions = scope.Resolve<ConfigurationOptions>();
            });

            return rootConfig;
        }


    }
}
