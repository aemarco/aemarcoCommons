using aemarcoCommons.Toolbox;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Spectre.Console.Cli;
using System;

namespace aemarcoCommons.ConsoleTools;

public static class BootstrapperExtensions
{

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



    public static IServiceCollection SetupConsoleTools(this IServiceCollection services)
    {
        services.SetupToolbox();

        return services;
    }




    public static ContainerBuilder SetupSerilogAsILogger(this ContainerBuilder builder)
    {
        var factory = new LoggerFactory(new ILoggerProvider[] { new SerilogLoggerProvider() });
        builder.SetupLoggerFactory(factory);
        return builder;
    }



    public static ITypeRegistrar ToTypeRegistrar(this ContainerBuilder builder) =>
        new AutofacTypeRegistrar(builder);


    //TODO create a ServiceCollection "ToTypeRegistrar"
    // we can wrap all stuff in SearchAndDestroy in a populate... and use the serviceCollection then.
}


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
        if (factory is null)
            throw new ArgumentNullException(nameof(factory));
        _builder.Register(_ => factory())
            .As(service)
            .SingleInstance();
    }

    public ITypeResolver Build() => new AutofacTypeResolver(_builder.Build());
}

public sealed class AutofacTypeResolver : ITypeResolver, IDisposable
{
    private readonly ILifetimeScope _scope;
    public AutofacTypeResolver(ILifetimeScope scope)
    {
        _scope = scope;
    }
    public object Resolve(Type type)
    {
        return _scope.TryResolve(type, out var instance)
            ? instance
            : null;
    }
    public void Dispose()
    {
        _scope?.Dispose();
    }
}







//public sealed class ServiceCollectionTypeRegistrar : ITypeRegistrar
//{
//    private readonly IServiceCollection _builder;

//    public ServiceCollectionTypeRegistrar(IServiceCollection builder)
//    {
//        _builder = builder;
//    }

//    public ITypeResolver Build()
//    {
//        return new ServiceCollectionTypeResolver(_builder.BuildServiceProvider());
//    }

//    public void Register(Type service, Type implementation)
//    {
//        _builder.AddSingleton(service, implementation);
//    }

//    public void RegisterInstance(Type service, object implementation)
//    {
//        _builder.AddSingleton(service, implementation);
//    }

//    public void RegisterLazy(Type service, Func<object> func)
//    {
//        if (func is null)
//        {
//            throw new ArgumentNullException(nameof(func));
//        }

//        _builder.AddSingleton(service, (provider) => func());
//    }
//}

//public sealed class ServiceCollectionTypeResolver : ITypeResolver, IDisposable
//{
//    private readonly IServiceProvider _provider;

//    public ServiceCollectionTypeResolver(IServiceProvider provider)
//    {
//        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
//    }

//    public object Resolve(Type type)
//    {
//        if (type == null)
//        {
//            return null;
//        }

//        return _provider.GetService(type);
//    }

//    public void Dispose()
//    {
//        if (_provider is IDisposable disposable)
//        {
//            disposable.Dispose();
//        }
//    }
//}