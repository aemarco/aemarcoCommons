using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox
{
    public static partial class BootstrapperExtensions
    {

        [Obsolete("Use IServiceCollection implementation")]
        public static ContainerBuilder SetupToolbox(this ContainerBuilder builder)
        {
            builder.Populate(new ServiceCollection().SetupToolbox());
            return builder;
        }

        [Obsolete("Use IServiceCollection implementation")]
        public static ContainerBuilder SetupLoggerFactory(this ContainerBuilder builder, ILoggerFactory factory)
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
