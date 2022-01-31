using aemarcoCommons.Toolbox;
using aemarcoCommons.WpfTools.Commands;
using aemarcoCommons.WpfTools.MonitorTools;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using System;
using System.Windows;

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



    public static class Bootstrapper
    {

        internal static ILifetimeScope RootScope { get; private set; }


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
            builder.SetupToolbox();
            var sc = new ServiceCollection();


            //* some common stuff
            builder.RegisterInstance(Application.Current.Dispatcher);
            builder.RegisterType<Random>().SingleInstance();

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
            sc.AddHttpClient();
            sc.AddHttpClient(nameof(WallpaperSetter));



            builder.Populate(sc);
            builder.RegisterBuildCallback(rootScope => RootScope = rootScope);

            return builder;
        }

        public static ContainerBuilder SetupSerilogAsILogger(this ContainerBuilder builder, out ILoggerFactory factory)
        {
            factory = new LoggerFactory(new ILoggerProvider[] { new SerilogLoggerProvider() });

            //logging
            builder.RegisterInstance(factory)
                .As<ILoggerFactory>()
                .SingleInstance();
            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

            return builder;
        }

    }

}
