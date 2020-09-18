using System;
using System.IO;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace aemarcoCommons.Toolbox.ConfigurationTools
{
    public static class Extensions
    {

        /// <summary>
        /// Register all necessary stuff to the Builder, so Setting-Classes can be injected later
        /// </summary>
        /// <param name="builder">builder used for registrations</param>
        /// <param name="configBuilder">config builder used for registrations</param>
        /// <param name="options">optional configuration for certain features</param>
        /// <returns>given builder, so that it can be chained</returns>
        public static ContainerBuilder AddConfigurationUtils(this ContainerBuilder builder,
            IConfigurationBuilder configBuilder,
            Action<ConfigurationOptions> options = null)
        {
            //set options
            var toolConfig = new ConfigurationOptions();
            options?.Invoke(toolConfig);
            SettingsBase.Options = toolConfig;

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
                    var filePath = type.GetSavePathForSetting(toolConfig.SettingsSaveDirectory);
                    configBuilder.AddJsonFile(filePath, true, true);
                }
            }

            //register IConfiguration stuff
            var rootConfig = configBuilder.Build();
            builder.RegisterInstance(rootConfig)
                .As<IConfiguration>()
                .As<IConfigurationRoot>()
                .SingleInstance();
            SettingsBase.ConfigRoot = rootConfig;

            return builder;
        }


        /// <summary>
        /// Gather absolute file Path which the type get saved to
        /// </summary>
        /// <param name="type">type for which the file Path should be returned</param>
        /// <param name="saveDirectory">directory path (same as provided in options)</param>
        /// <returns>absolute file path to the saved configuration file</returns>
        public static string GetSavePathForSetting(this Type type, string saveDirectory)
        {
            return Path.Combine(saveDirectory, $"appsettings.{type.Name}.json");
        }


        /// <summary>
        /// Performs string transformations on given objects string properties with ProtectedAttribute defined
        /// </summary>
        /// <param name="obj">object which needs transformation</param>
        /// <param name="transform">transformation which should be performed</param>
        internal static void ProtectedTransformObject(this object obj, Func<string, string> transform)
        {
            foreach (var propInfo in obj.GetType().GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(ProtectedAttribute)))
                .Where(x => x.PropertyType == typeof(string))
                .Where(x => x.CanRead && x.CanWrite))
            {
                propInfo.SetValue(
                    obj,
                    transform((string)propInfo.GetValue(obj)));
            }

            foreach (var propInfo in obj.GetType().GetProperties()
                .Where(x => x.PropertyType.IsClass && x.PropertyType != typeof(string)))
            {
                propInfo.GetValue(obj)
                    .ProtectedTransformObject(transform);
            }
        }

    }
}