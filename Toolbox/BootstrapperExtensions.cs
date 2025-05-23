﻿using aemarcoCommons.Toolbox.GeoTools;
using aemarcoCommons.Toolbox.NetworkTools;
using aemarcoCommons.Toolbox.Oidc;
using aemarcoCommons.Toolbox.SecurityTools;
using aemarcoCommons.Toolbox.SerializationTools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using System.Reflection;

namespace aemarcoCommons.Toolbox
{
    public static class BootstrapperExtensions
    {

        public static IConfigurationBuilder ConfigAppsettings(this IConfigurationBuilder builder, string environmentName = null)
        {

            environmentName = environmentName
                              ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                              ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                              ?? Environment.GetEnvironmentVariable("ENVIRONMENT")
                              ?? "Production";

            builder
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true);

            if (environmentName == "Development")
            {
                //https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows
                var assembly = Assembly.GetEntryAssembly()
                               ?? throw new Exception("Could not determine entry assembly.");
                builder
                    .AddUserSecrets(assembly);
            }
            return builder;
        }

        public static IServiceCollection SetupToolbox(this IServiceCollection sc)
        {
            //services
            sc.AddSingleton<Random>();
            sc.AddSingleton(typeof(ITypeToFileStore<>), typeof(JsonTypeToFileStore<>));
            sc.AddTransient<IEmbeddedResourceQuery, EmbeddedResourceQuery>();

            //SecurityTools
            sc.AddTransient<VirusScanService>();

            sc.AddSingleton<GeoService>();


            //polly
            var policyRegistry = sc.AddPolicyRegistry();
            if (!policyRegistry.ContainsKey("HttpRetry"))
            {
                policyRegistry.Add(
                    "HttpRetry",
                    HttpPolicyExtensions.HandleTransientHttpError()
                        .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)));
            }
            if (!policyRegistry.ContainsKey("HttpCircuitBreaker"))
            {
                policyRegistry.Add(
                    "HttpCircuitBreaker",
                    HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .CircuitBreakerAsync(
                            handledEventsAllowedBeforeBreaking: 10,
                            durationOfBreak: TimeSpan.FromMinutes(5)));
            }


            //httpclient
            sc.AddTransient<RateLimitingPerHostHandler>();
            sc.AddTransient<IgnoreServerCertificateHandler>();
            sc.AddTransient<OidcTokenRenewalHandler>();
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
