using aemarcoCommons.Toolbox;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace aemarcoCommons.ConsoleTools;

public static partial class BootstrapperExtensions
{

    public static IServiceCollection SetupConsoleTools(this IServiceCollection services)
    {
        services.SetupToolbox();



        return services;
    }

    //Microsoft.Extensions.DependencyInjection
    public static ITypeRegistrar ToTypeRegistrar(this IServiceCollection services) =>
        new SpectreInfrastructure.ServiceCollectionTypeRegistrar(services);

}