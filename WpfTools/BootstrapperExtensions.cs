using aemarcoCommons.Toolbox;
using aemarcoCommons.WpfTools.BaseNav;
using aemarcoCommons.WpfTools.Commands;
using aemarcoCommons.WpfTools.Dialogs;
using aemarcoCommons.WpfTools.WindowStuff;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;


namespace aemarcoCommons.WpfTools;

/// <summary>
/// Implement to get the class being registered as SingleInstance
/// </summary>
public interface ISingleton;

/// <summary>
/// Implement to get the class being registered as InstancePerDependency
/// </summary>
public interface ITransient;



public static class BootstrapperExtensions
{

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
    /// <param name="registerWindows"></param>
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



        sc.AddTransient<IWindowService, WindowService>();
        IWindowService.RegisterDialog<AskInputDialogView, AskInputDialogViewModel>();


        sc.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);



        //var waitAndRetry = HttpPolicyExtensions
        //    .HandleTransientHttpError()
        //    .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1));
        //var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));


        //#pragma warning disable CS0612 // Type or member is obsolete
        //        sc.AddHttpClient(nameof(WallpaperSetter), c =>
        //#pragma warning restore CS0612 // Type or member is obsolete
        //            {
        //                c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.88 Safari/537.36");
        //            })
        //            .AddPolicyHandler(waitAndRetry)
        //            .AddPolicyHandler(timeoutPolicy);


        sc.AddHostedService<NavigationSetupService>();

        return sc;
    }

}