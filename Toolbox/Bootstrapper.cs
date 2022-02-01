using aemarcoCommons.Toolbox.GeoTools;
using aemarcoCommons.Toolbox.SerializationTools;
using Autofac;
using System;

namespace aemarcoCommons.Toolbox
{
    public static class Bootstrapper
    {
        internal static ILifetimeScope RootScope { get; private set; }
        public static ContainerBuilder SetupToolbox(this ContainerBuilder builder)
        {

            builder.RegisterType<Random>().SingleInstance();

            //* Toolbox stuff
            builder.RegisterGeneric(typeof(JsonTypeToFileStore<>))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<GeoService>();
            builder.RegisterType<GeoServiceSettings>().AsImplementedInterfaces();


            builder.RegisterBuildCallback(scope => RootScope = scope);
            return builder;
        }


    }
}
