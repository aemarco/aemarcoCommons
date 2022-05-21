using aemarcoCommons.Toolbox.GeoTools;
using aemarcoCommons.Toolbox.SerializationTools;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace aemarcoCommons.Toolbox
{
    public static class Bootstrapper
    {
        internal static ILifetimeScope RootScope { get; private set; }
        public static ContainerBuilder SetupToolbox(this ContainerBuilder builder)
        {
            var sc = new ServiceCollection();


            builder.RegisterType<Random>().SingleInstance();

            //* Toolbox stuff
            builder.RegisterGeneric(typeof(JsonTypeToFileStore<>))
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<EmbeddedResourceQuery>()
                .AsImplementedInterfaces()
                .SingleInstance();




            builder.RegisterType<GeoService>();
            //in case AppConfiguration is not used from toolbox
            //somebody may override this still
            builder.RegisterType<GeoServiceSettings>()
                .AsImplementedInterfaces()
                .IfNotRegistered(typeof(IGeoServiceSettings));

            var waitAndRetry = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1));
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
            sc.AddHttpClient(nameof(GeoService))
                .AddPolicyHandler(waitAndRetry)
                .AddPolicyHandler(timeoutPolicy)
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            (r, c, ch, e) => true
                    };
                });

            builder.Populate(sc);
            builder.RegisterBuildCallback(scope => RootScope = scope);
            return builder;
        }


    }
}
