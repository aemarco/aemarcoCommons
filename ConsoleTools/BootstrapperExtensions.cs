using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace aemarcoCommons.ConsoleTools;

public static class BootstrapperExtensions
{

    //Microsoft.Extensions.DependencyInjection
    public static ITypeRegistrar ToTypeRegistrar(this IServiceCollection services) =>
        new SpectreInfrastructure.ServiceCollectionTypeRegistrar(services);

}