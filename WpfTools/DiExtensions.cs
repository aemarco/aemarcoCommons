using System;
using System.Windows;
using aemarcoCommons.Toolbox.ConfigurationTools;
using aemarcoCommons.Toolbox.SerializationTools;
using aemarcoCommons.WpfTools.Commands;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace aemarcoCommons.WpfTools
{

    /// <summary>
    /// Implement to get the class being registered as SingleInstance
    /// </summary>
    public interface ISingleton { }
    
    /// <summary>
    /// Implement to get the class being registered as InstancePerDependency
    /// </summary>
    public interface ITransient { }



    public static class DiExtensions
    {
        public static IConfigurationBuilder ConfigAppsettings(this IConfigurationBuilder builder)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            return builder
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true);
        }

        /// <summary>
        /// Call this, to register
        /// * some common stuff
        /// * Toolbox stuff
        /// * WpfTools stuff
        /// </summary>
        /// <param name="builder">builder to register to</param>
        /// <returns>builder with registrations</returns>
        public static ContainerBuilder SetupWpfTools(this ContainerBuilder builder)
        {
            //* some common stuff
            builder.RegisterInstance(Application.Current.Dispatcher);
            builder.RegisterType<Random>().SingleInstance();

            //* Toolbox stuff
            builder.RegisterGeneric(typeof(JsonTypeToFileStore<>))
                .AsImplementedInterfaces()
                .SingleInstance();


            //* WpfTools stuff
            //--> windows getting registered
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => t.IsSubclassOf(typeof(Window)))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();
            //--stuff get registered which implement interfaces (overrides window registrations)
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => typeof(ISingleton).IsAssignableFrom(t))
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => typeof(ITransient).IsAssignableFrom(t))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();
            

            builder.RegisterGeneric(typeof(OpenWindowCommand<>));
            builder.RegisterGeneric(typeof(OpenDialogCommand<>));

            builder.RegisterBuildCallback(rootScope => RootScope = rootScope);

            return builder;
        }

        internal static ILifetimeScope RootScope { get; private set; }

    }

}
