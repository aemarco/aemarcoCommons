using aemarcoCommons.ToolboxAppOptions.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace aemarcoCommons.ToolboxAppOptions
{
    public static class AutoFacExtensions
    {

        [Obsolete("Autofac support will be removed")]
        public static ContainerBuilder AddConfigOptionsUtils(
            this ContainerBuilder builder,
            IConfigurationRoot config,
            Action<ConfigurationOptionsBuilder> options = null)
        {
            builder.Populate(
                new ServiceCollection()
                    .AddConfigOptionsUtils(
                        config,
                        options));

            builder.RegisterBuildCallback(scope =>
            {
                if (scope.TryResolve<StartupValidationService>(out var startValidator))
                {
                    startValidator.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
                }
            });

            return builder;
        }
    }
}