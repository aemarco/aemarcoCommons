using System;
using System.Windows;
using Autofac;
using Toolbox.ConfigurationTools;
using Toolbox.SerializationTools;

namespace WpfTools
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
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => t.IsSubclassOf(typeof(SettingsBase)))
                .AsSelf();
            builder.RegisterGeneric(typeof(JsonTypeToFileStore<>))
                .AsImplementedInterfaces()
                .SingleInstance();


            //* WpfTools stuff
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
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => t.IsSubclassOf(typeof(Window)))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            return builder;
        }

    }

}
