using System;
using aemarcoCommons.Toolbox.GeoTools;
using aemarcoCommons.Toolbox.SerializationTools;
using Autofac;

namespace aemarcoCommons.Toolbox
{
    public static class Bootstrapper
    {
        public static ContainerBuilder SetupToolbox(this ContainerBuilder builder)
        {

            builder.RegisterType<Random>().As<Random>().SingleInstance();

            //* Toolbox stuff
            builder.RegisterGeneric(typeof(JsonTypeToFileStore<>))
                .AsImplementedInterfaces()
                .SingleInstance();
           
            builder.RegisterType<GeoService>();
            builder.RegisterType<GeoServiceSettings>().AsImplementedInterfaces();

            return builder;
        }


    }
}
