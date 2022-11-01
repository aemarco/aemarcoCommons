using aemarcoCommons.Toolbox;
using aemarcoCommons.WpfTools.Commands;
using aemarcoCommons.WpfTools.MonitorTools;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Serilog.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using Policy = Polly.Policy;

namespace aemarcoCommons.WpfTools;

/// <summary>
/// Implement to get the class being registered as SingleInstance
/// </summary>
public interface ISingleton { }

/// <summary>
/// Implement to get the class being registered as InstancePerDependency
/// </summary>
public interface ITransient { }



public static class BootstrapperExtensions
{

    internal static ILifetimeScope RootScope { get; private set; }

    /// <summary>
    /// Call this, to register
    /// * some common stuff
    /// * Toolbox stuff
    /// * WpfTools stuff
    /// </summary>
    /// <param name="builder">builder to register to</param>
    /// <param name="externals">externally owned objects</param>
    /// <returns>builder with registrations</returns>
    public static ContainerBuilder SetupWpfTools(this ContainerBuilder builder, params object[] externals)
    {
        foreach (var external in externals)
        {
            builder.RegisterInstance(external)
                .AsSelf()
                .ExternallyOwned();
        }

        var sc = new ServiceCollection()
            .SetupToolbox();


        //* some common stuff
        builder.RegisterInstance(Application.Current.Dispatcher);

        //* WpfTools stuff
        //--> windows getting registered

        var shortestNameSpace = Assembly.GetEntryAssembly()!
            .GetTypes()
            .Select(x => x.Namespace)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OrderBy(x => x!.Length)
            .First(); //now only registers consumers windows
        builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
            .Where(x => x.Namespace?.StartsWith(shortestNameSpace) ?? false)
            .Where(t => t.IsSubclassOf(typeof(Window)))
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerDependency();
        //--stuff get registered which implement interfaces (overrides window registrations)
        builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
            .Where(t => typeof(ISingleton).IsAssignableFrom(t))
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
        builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
            .Where(t => typeof(ITransient).IsAssignableFrom(t))
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerDependency();

        builder.RegisterGeneric(typeof(OpenWindowCommand<>))
            .FindConstructorsWith(t => new[] { t.GetConstructor(new[] { typeof(ILifetimeScope) }) });
        builder.RegisterGeneric(typeof(OpenDialogCommand<>))
            .FindConstructorsWith(t => new[] { t.GetConstructor(new[] { typeof(ILifetimeScope) }) });
        builder.RegisterGeneric(typeof(ShowWindowCommand<>))
            .FindConstructorsWith(t => new[] { t.GetConstructor(new[] { typeof(ILifetimeScope) }) });


        var waitAndRetry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1));
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));


        sc.AddHttpClient(nameof(WallpaperSetter), c =>
            {
                c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.88 Safari/537.36");
            })
            .AddPolicyHandler(waitAndRetry)
            .AddPolicyHandler(timeoutPolicy);

        builder.Populate(sc);
        builder.RegisterBuildCallback(rootScope => RootScope = rootScope);

        return builder;
    }

    public static IServiceCollection SetupWpfTools(this IServiceCollection sc, params object[] externals)
    {

        foreach (var external in externals)
        {
            sc.AddSingleton(external);
        }

        sc.SetupToolbox();

        //* some common stuff
        sc.AddSingleton(Application.Current.Dispatcher);

        //* WpfTools stuff
        //--> windows getting registered
        var shortestNameSpace = Assembly.GetEntryAssembly()!
            .GetTypes()
            .Select(x => x.Namespace)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OrderBy(x => x!.Length)
            .First(); //now only registers consumers windows

        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.Namespace?.StartsWith(shortestNameSpace) ?? false)
            .ToList()
            .ForEach(t =>
            {
                if (t.IsSubclassOf(typeof(Window)))
                {
                    sc.AddTransient(t);
                    foreach (var i in t.GetInterfaces())
                        sc.AddTransient(i, t);
                }
                else if (typeof(ISingleton).IsAssignableFrom(t))
                {
                    sc.AddSingleton(t);
                    foreach (var i in t.GetInterfaces())
                        sc.AddSingleton(i, sp => sp.GetService(t));
                }
                else if (typeof(ITransient).IsAssignableFrom(t))
                {
                    sc.AddTransient(t);
                    foreach (var i in t.GetInterfaces())
                        sc.AddTransient(i, t);
                }
            });


        sc.AddTransient(typeof(OpenWindowCommand<>));
        sc.AddTransient(typeof(OpenDialogCommand<>));
        sc.AddTransient(typeof(ShowWindowCommand<>));


        var waitAndRetry = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1));
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));


        sc.AddHttpClient(nameof(WallpaperSetter), c =>
            {
                c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.88 Safari/537.36");
            })
            .AddPolicyHandler(waitAndRetry)
            .AddPolicyHandler(timeoutPolicy);

        return sc;
    }


    public static ContainerBuilder SetupSerilogAsILogger(this ContainerBuilder builder)
    {
        var factory = new LoggerFactory(new ILoggerProvider[] { new SerilogLoggerProvider() });
        builder.SetupLoggerFactory(factory);
        return builder;
    }

}