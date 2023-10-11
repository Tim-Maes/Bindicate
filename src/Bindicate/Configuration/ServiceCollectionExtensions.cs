using Bindicate.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Bindicate.Configuration;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers services decorated with <see cref="BaseServiceAttribute"/> derivatives in the specified assembly.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="assembly">The assembly to scan for types decorated with service registration attributes.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a type does not implement the specified service interface.</exception>
    public static IServiceCollection AddBindicate(this IServiceCollection services, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
        {
            var registerAttributes = type.GetCustomAttributes(typeof(BaseServiceAttribute), false)
                                         .Cast<BaseServiceAttribute>();

            foreach (var attr in registerAttributes)
            {
                var serviceType = attr.ServiceType ?? type;

                if (type.GetInterfaces().Contains(serviceType) || type == serviceType)
                {
                    switch (attr.Lifetime)
                    {
                        case Lifetime.Scoped:
                            RegisterService(serviceType, type, (s, t) => services.AddScoped(s, t));
                            break;
                        case Lifetime.TryAddScoped:
                            RegisterService(serviceType, type, (s, t) => services.TryAddScoped(s, t));
                            break;
                        case Lifetime.Singleton:
                            RegisterService(serviceType, type, (s, t) => services.AddSingleton(s, t));
                            break;
                        case Lifetime.TryAddSingleton:
                            RegisterService(serviceType, type, (s, t) => services.TryAddSingleton(s, t));
                            break;
                        case Lifetime.Transient:
                            RegisterService(serviceType, type, (s, t) => services.AddTransient(s, t));
                            break;
                        case Lifetime.TryAddTransient:
                            RegisterService(serviceType, type, (s, t) => services.TryAddTransient(s, t));
                            break;
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Type {type.FullName} does not implement {serviceType.FullName}");
                }
            }
        }

        return services;
    }

    private static void RegisterService(Type serviceType, Type implementationType, Action<Type, Type?> registrationMethod)
    {
        if (serviceType == implementationType)
            registrationMethod(implementationType, implementationType);
        else
            registrationMethod(serviceType, implementationType);
    }
}