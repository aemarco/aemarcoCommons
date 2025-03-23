using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace aemarcoCommons.ConsoleTools.SpectreInfrastructure;

public sealed class AppTypeRegistrar : ITypeRegistrar
{
    private readonly HostApplicationBuilder _builder;
    internal IHost Host { get; private set; }

    public AppTypeRegistrar(HostApplicationBuilder builder) =>
        _builder = builder;

    public void Register(Type service, Type implementation) =>
        _builder.Services.AddScoped(service, implementation);

    public void RegisterInstance(Type service, object implementation) =>
        _builder.Services.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _builder.Services.AddScoped(service, _ => factory());
    }

    public ITypeResolver Build()
    {
        Host = _builder.Build();
        return new ServiceCollectionTypeResolver(Host.Services.CreateScope().ServiceProvider);
    }

}