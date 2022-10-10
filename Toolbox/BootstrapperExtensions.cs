using aemarcoCommons.Toolbox.GeoTools;
using aemarcoCommons.Toolbox.NetworkTools;
using aemarcoCommons.Toolbox.Oidc;
using aemarcoCommons.Toolbox.SecurityTools;
using aemarcoCommons.Toolbox.SerializationTools;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
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


        public static IServiceCollection SetupToolbox(this IServiceCollection sc) =>
            sc
                .SetupServices()
                .SetupPollyPolicies()
                .SetupHttpClientStuff();

        private static IServiceCollection SetupServices(this IServiceCollection sc)
        {
            sc.AddSingleton<Random>();
            sc.AddSingleton(typeof(ITypeToFileStore<>), typeof(JsonTypeToFileStore<>));
            sc.AddTransient<IEmbeddedResourceQuery, EmbeddedResourceQuery>();

            sc.AddTransient<VirusScanService>();
            sc.AddTransient<IGeoServiceSettings, GeoServiceSettings>();
            sc.AddTransient<GeoService>();

            return sc;
        }

        private static IServiceCollection SetupPollyPolicies(this IServiceCollection sc)
        {
            var policyRegistry = sc.AddPolicyRegistry();

            policyRegistry.Add(
                "HttpRetry",
                HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));

            policyRegistry.Add(
                "HttpCircuitBreaker",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: 10,
                        durationOfBreak: TimeSpan.FromMinutes(5)));

            return sc;
        }

        private static IServiceCollection SetupHttpClientStuff(this IServiceCollection sc)
        {
            sc.AddTransient<RateLimitingPerHostHandler>();
            sc.AddTransient<OidcTokenRenewalHandler>();
            sc.AddTransient<IgnoreServerCertificateHandler>();
            sc.AddSingleton<OidcTokenRenewalHandlerHelper>();

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
            sc.AddHttpClient(nameof(GeoService))
                .AddPolicyHandlerFromRegistry("HttpRetry")
                .AddPolicyHandler(timeoutPolicy)
                .ConfigurePrimaryHttpMessageHandler<IgnoreServerCertificateHandler>();

            return sc;
        }


    }
}
