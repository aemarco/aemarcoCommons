using aemarcoCommons.Toolbox.GeoTools;
using aemarcoCommons.Toolbox.SecurityTools;
using aemarcoCommons.Toolbox.SerializationTools;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace aemarcoCommons.Toolbox
{
    public static class BootstrapperExtensions
    {

        public static IConfigurationBuilder ConfigAppsettings(this IConfigurationBuilder builder)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            return builder
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true);
        }


        internal static ILifetimeScope RootScope { get; private set; }
        public static ContainerBuilder SetupToolbox(this ContainerBuilder builder)
        {
            var sc = new ServiceCollection();


            builder.RegisterType<Random>().SingleInstance();

            //* Toolbox stuff

            builder.RegisterType<VirusScanService>();


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

        public static ContainerBuilder SetupLoggerFactory(
            this ContainerBuilder builder,
            ILoggerFactory factory)
        {
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
