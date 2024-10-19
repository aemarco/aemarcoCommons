using Spectre.Console.Cli;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools.SpectreInfrastructure;

//https://spectreconsole.net/cli/commandapp
//https://github.com/spectreconsole/examples/blob/main/examples/Cli/Injection/Infrastructure/TypeResolver.cs

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