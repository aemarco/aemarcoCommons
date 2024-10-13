using aemarcoCommons.Toolbox;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Spectre.Console.Cli;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.ConsoleTools;
public static partial class BootstrapperExtensions
{

    [Obsolete("Use IServiceCollection implementation")]
    public static ContainerBuilder SetupConsoleTools(this ContainerBuilder builder)
    {
        var sc = new ServiceCollection()
            .SetupToolbox();
        //register console stuff here

        //builder.RegisterInstance(new CancelKeyPressHandler());
        //builder.Register(scope => (CancellationToken)scope.Resolve<CancelKeyPressHandler>())
        //    .SingleInstance();

        builder.Populate(sc);

        return builder;
    }

    [Obsolete("Use IServiceCollection implementation")]
    public static ContainerBuilder SetupSerilogAsILogger(this ContainerBuilder builder)
    {
        var factory = new LoggerFactory(new ILoggerProvider[] { new SerilogLoggerProvider() });
        builder.SetupLoggerFactory(factory);
        return builder;
    }


    //Autofac
    [Obsolete("Use IServiceCollection implementation")]
    public static ITypeRegistrar ToTypeRegistrar(this ContainerBuilder builder) =>
        new AutofacTypeRegistrar(builder);



}

[Obsolete("Use IServiceCollection implementation")]
public sealed class AutofacTypeRegistrar : ITypeRegistrar
{
    private readonly ContainerBuilder _builder;

    public AutofacTypeRegistrar(ContainerBuilder builder) =>
        _builder = builder;

    public void Register(Type service, Type implementation) =>
        _builder.RegisterType(implementation)
            .As(service)
            .SingleInstance();

    public void RegisterInstance(Type service, object implementation) =>
        _builder.RegisterInstance(implementation)
            .As(service)
            .SingleInstance();

    public void RegisterLazy(Type service, Func<object> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _builder.Register(_ => factory())
            .As(service)
            .SingleInstance();
    }

    public ITypeResolver Build() => new AutofacTypeResolver(_builder.Build());
}
[Obsolete("Use IServiceCollection implementation")]
public sealed class AutofacTypeResolver : ITypeResolver, IDisposable
{
    private readonly ILifetimeScope _scope;
    public AutofacTypeResolver(ILifetimeScope scope) => _scope = scope;

    public object Resolve(Type type) =>
        type is null
            ? null
            : _scope.TryResolve(type, out var instance)
                ? instance
                : null;

    public void Dispose() => _scope?.Dispose();
}