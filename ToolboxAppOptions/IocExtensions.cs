using aemarcoCommons.ToolboxAppOptions.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;

namespace aemarcoCommons.ToolboxAppOptions
{
    public static class IocExtensions
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
            return builder;
        }

        public static IServiceCollection AddConfigOptionsUtils(
            this IServiceCollection sc,
            IConfigurationRoot config,
            Action<ConfigurationOptionsBuilder> options = null)
        {
            //register IConfiguration stuff
            sc.AddSingleton(config);
            sc.AddSingleton<IConfiguration>(sp => sp.GetRequiredService<IConfigurationRoot>());

            //register tool config
            var toolConfigBuilder = new ConfigurationOptionsBuilder();
            options?.Invoke(toolConfigBuilder);
            var toolConfig = toolConfigBuilder.Build();
            sc.AddSingleton(toolConfig);



            foreach (var type in AppDomain.CurrentDomain
                         .GetAssemblies()
                         .SelectMany(x => x.GetTypes())
                         .Where(x => x.IsSubclassOf(typeof(SettingsBase))))
            {

                //setup options build pipeline
                sc.ConfigureOptions(typeof(AppOptionFactory<>).MakeGenericType(type));

                //register type as self resolved through options
                Type optionsType = typeof(IOptions<>).MakeGenericType(type);
                sc.AddSingleton(type, sp =>
                    optionsType.InvokeMember(
                        "Value",
                        BindingFlags.GetProperty,
                        null,
                        sp.GetRequiredService(optionsType),
                        Array.Empty<object>()));

                //register 
                var interfaces = type.GetInterfaces();
                foreach (Type interfaceType in interfaces)
                {
                    sc.AddSingleton(interfaceType, s => s.GetRequiredService(type));

                }


            }
            return sc;
        }
    }
}
