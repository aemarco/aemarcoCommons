using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace aemarcoCommons.ConsoleTools.SpectreInfrastructure;

//https://spectreconsole.net/cli/commandapp
//https://github.com/spectreconsole/examples/blob/main/examples/Cli/Injection/Infrastructure/TypeRegistrar.cs

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