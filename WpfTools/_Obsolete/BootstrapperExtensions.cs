using aemarcoCommons.Toolbox;
using aemarcoCommons.WpfTools.BaseNav;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.WpfTools;

public static partial class BootstrapperExtensions
{

    /// <summary>
    /// Call this, to register
    /// * some common stuff
    /// * Toolbox stuff
    /// * WpfTools stuff
    /// </summary>
    /// <param name="builder">builder to register to</param>
    /// <param name="externals">externally owned objects</param>
    /// <returns>builder with registrations</returns>
    [Obsolete("Use IServiceCollection implementation")]
    public static ContainerBuilder SetupWpfTools(this ContainerBuilder builder, params object[] externals)
    {
        builder.Populate(new ServiceCollection().SetupWpfTools(externals: externals));
        builder.RegisterBuildCallback(rootScope =>
        {
#pragma warning disable CsWinRT1030
            BaseNavWindowViewModel.ServiceProvider = new AutofacServiceProvider(rootScope);
#pragma warning restore CsWinRT1030
        });
        return builder;
    }


    [Obsolete("Use IServiceCollection implementation")]
    public static ContainerBuilder SetupSerilogAsILogger(this ContainerBuilder builder)
    {
        var factory = new LoggerFactory(new ILoggerProvider[] { new SerilogLoggerProvider() });
        builder.SetupLoggerFactory(factory);
        return builder;
    }


    [Obsolete("Navigation setup is now done automatically")]
    public static IServiceProvider SetupServiceProviderForWpfTools(this IServiceProvider serviceProvider)
    {
        BaseNavWindowViewModel.ServiceProvider = serviceProvider;
        return serviceProvider;
    }


}
