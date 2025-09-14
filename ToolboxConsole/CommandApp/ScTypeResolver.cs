using Spectre.Console.Cli;
using System;

namespace aemarcoCommons.ToolboxConsole.CommandApp;

//https://spectreconsole.net/cli/commandapp
//https://github.com/spectreconsole/examples/blob/main/examples/Cli/Injection/Infrastructure/TypeResolver.cs

public sealed class ScTypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;
    public ScTypeResolver(IServiceProvider provider) => _provider = provider;

    public object? Resolve(Type? type) =>
        type != null
            ? _provider.GetService(type)
            : null;

    public void Dispose()
    {
        if (_provider is IDisposable disposable)
            disposable.Dispose();
    }
}