using aemarcoCommons.Toolbox;
using aemarcoCommons.WpfTools.Commands;
using aemarcoCommons.WpfTools.MonitorTools;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Serilog.Extensions.Logging;
using System;
using System.Net.Http;
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



    public static class BootstrapperExtensions
    {

        internal static ILifetimeScope RootScope { get; private set; }


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
            var sc = new ServiceCollection()
                .SetupToolbox();


            //* some common stuff
            builder.RegisterInstance(Application.Current.Dispatcher);

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



            var waitAndRetry = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1));
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));


            sc.AddHttpClient(nameof(WallpaperSetter))
                .AddPolicyHandler(waitAndRetry)
                .AddPolicyHandler(timeoutPolicy);

            builder.Populate(sc);
            builder.RegisterBuildCallback(rootScope => RootScope = rootScope);

            return builder;
        }

        public static ContainerBuilder SetupSerilogAsILogger(this ContainerBuilder builder)
        {
            var factory = new LoggerFactory(new ILoggerProvider[] { new SerilogLoggerProvider() });
            builder.SetupLoggerFactory(factory);
            return builder;
        }

    }

}
