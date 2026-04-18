using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace aemarcoCommons.ConsoleTools;

public static class BootstrapperExtensions
{

    //Microsoft.Extensions.DependencyInjection
    [Obsolete("Use aemarcoCommons.ToolboxConsole instead.")]
    public static ITypeRegistrar ToTypeRegistrar(this IServiceCollection services) =>
        new SpectreInfrastructure.ServiceCollectionTypeRegistrar(services);

}