using aemarcoCommons.ToolboxAppOptions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;


//TODO: Save
//TODO: Auto-Save
//TODO: Protected
//TODO: array merging?
//TODO: Validate
//TODO: Validate maybe fluent
//TODO: Validate on Start


namespace aemarcoCommons.ToolboxAppOptions
{
    public static class IocExtensions
    {

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
            }


            //TODO: work on ValidateOnStart
            //sc.AddOptions<Random>().ValidateOnStart();
            //project: Microsoft.Extensions.Hosting
            //rather not including hosting, but ValidateOnStart could eventually be done otherwise?!

            return sc;
        }
    }
}
