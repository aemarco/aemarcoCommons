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
        new ServiceCollectionTypeRegistrar(services);

}

public sealed class ServiceCollectionTypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _builder;

    public ServiceCollectionTypeRegistrar(IServiceCollection builder) =>
        _builder = builder;

    public void Register(Type service, Type implementation) =>
        _builder.AddScoped(service, implementation);

    public void RegisterInstance(Type service, object implementation) =>
        _builder.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _builder.AddScoped(service, _ => factory());
    }

    public ITypeResolver Build() => new ServiceCollectionTypeResolver(_builder.BuildServiceProvider());
}
public sealed class ServiceCollectionTypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;
    public ServiceCollectionTypeResolver(IServiceProvider provider) => _provider = provider;

    public object Resolve(Type type) =>
        type != null
            ? _provider.GetService(type)
            : null;

    public void Dispose()
    {
        if (_provider is IDisposable disposable)
            disposable.Dispose();
    }
}