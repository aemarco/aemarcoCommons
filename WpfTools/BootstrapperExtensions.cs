using aemarcoCommons.Toolbox;
using aemarcoCommons.WpfTools.Commands;
using aemarcoCommons.WpfTools.MonitorTools;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
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

#nullable enable

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
        builder.Populate(new ServiceCollection().SetupWpfTools(externals: externals));
        builder.RegisterBuildCallback(rootScope =>
        {
            ServiceProvider = new AutofacServiceProvider(rootScope);
        });
        return builder;
    }


    /// <summary>
    /// Call this, to register
    /// * some common stuff
    /// * Toolbox stuff
    /// * WpfTools stuff
    /// Once you have the IServiceProvider, also call
    /// IServiceProvider.SetupServiceProviderForWpfTools();
    /// 
    /// </summary>
    /// <param name="sc">IServiceCollection to register to</param>
    /// <param name="externals">externally owned objects</param>
    /// <returns>IServiceCollection for chaining</returns>
    public static IServiceCollection SetupWpfTools(
        this IServiceCollection sc,
        bool registerWindows = true,
        params object[] externals)
    {

        foreach (var external in externals)
        {
            sc.AddSingleton(external.GetType(), external);
        }

        sc.SetupToolbox();

        //* some common stuff
        sc.AddSingleton(Application.Current.Dispatcher);


        //* WpfTools stuff
        //--> windows getting registered
        var shortestAppNamespace = Assembly.GetEntryAssembly()!
            .GetTypes()
            .Select(x => x.Namespace)
            .OfType<string>()
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OrderBy(x => x.Length)
            .First(); //now only registers consumers windows


        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.IsAbstract)
            .ToList()
            .ForEach(t =>
            {
                if ((t.Namespace?.StartsWith(shortestAppNamespace) ?? false) &&
                    t.IsSubclassOf(typeof(Window)) &&
                    registerWindows)
                {
                    sc.AddTransient(t);
                    foreach (var i in t.GetInterfaces())
                        sc.AddTransient(i, t);
                }
                else if (typeof(ISingleton).IsAssignableFrom(t))
                {
                    sc.AddSingleton(t);
                    foreach (var i in t.GetInterfaces())
                        sc.AddSingleton(i, sp => sp.GetRequiredService(t));
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

        sc.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

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
    public static IServiceProvider SetupServiceProviderForWpfTools(this IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        return serviceProvider;
    }






    public static ContainerBuilder SetupSerilogAsILogger(this ContainerBuilder builder)
    {
        var factory = new LoggerFactory(new ILoggerProvider[] { new SerilogLoggerProvider() });
        builder.SetupLoggerFactory(factory);
        return builder;
    }




    internal static IServiceProvider? ServiceProvider { get; private set; }

}