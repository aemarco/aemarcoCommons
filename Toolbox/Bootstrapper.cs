using aemarcoCommons.Toolbox.GeoTools;
using aemarcoCommons.Toolbox.SerializationTools;
using Autofac;

namespace aemarcoCommons.Toolbox
{
    public static class Bootstrapper
    {
        public static ContainerBuilder SetupToolbox(this ContainerBuilder builder)
        {
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
