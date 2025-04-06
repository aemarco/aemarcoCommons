using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;


namespace aemarcoCommons.Toolbox.Ioc
{
    //https://github.com/dotnet/runtime/issues/36021

    /// <summary>
    /// Helps register decorator implementations that wrap existing ones in the container.
    /// </summary>
    public static class DecoratorRegistrationExtensions
    {

        public static void Decorate<TService, TDecorator>(this IServiceCollection services)
            where TService : class
            where TDecorator : class, TService
        {
            var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(TService));

            // check it´s valid
            if (descriptor == null)
                throw new InvalidOperationException($"{typeof(TService).Name} is not registered");

            // create the object factory for our decorator type,
            // specifying that we will supply TInterface explicitly
            var objectFactory = ActivatorUtilities.CreateFactory(
                typeof(TDecorator),
                new[] { typeof(TService) });

            // replace the existing registration with one
            // that passes an instance of the existing registration
            // to the object factory for the decorator
            services.Replace(ServiceDescriptor.Describe(
                typeof(TService),
                sp => objectFactory(
                    sp,
                    new[]
                    {
                        descriptor.ImplementationInstance
                            ?? (descriptor.ImplementationFactory != null
                                ? descriptor.ImplementationFactory(sp)
                                : descriptor.ImplementationType != null
                                    ? ActivatorUtilities.GetServiceOrCreateInstance(sp, descriptor.ImplementationType)
                                    : throw new Exception("Unreachable"))
                    }),
                descriptor.Lifetime)
            );
        }
    }
}
