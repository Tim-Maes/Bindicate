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
        var types = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract);

        foreach (var type in types)
        {
            var registerAttributes = type.GetCustomAttributes(typeof(BaseServiceAttribute), false)
                                         .Cast<BaseServiceAttribute>();

            foreach (var attr in registerAttributes)
            {
                RegisterServiceBasedOnLifetime(services, type, attr);
            }
        }

        return services;
    }

    private static void RegisterServiceBasedOnLifetime(IServiceCollection services, Type type, BaseServiceAttribute attr)
    {
        var serviceType = attr.ServiceType ?? type;

        if (type.GetInterfaces().Contains(serviceType) || type == serviceType)
        {
            switch (attr.Lifetime)
            {
                case Lifetime.Scoped:
                    services.AddScoped(serviceType, type);
                    break;
                case Lifetime.TryAddScoped:
                    services.TryAddScoped(serviceType, type);
                    break;
                case Lifetime.Singleton:
                    services.AddSingleton(serviceType, type);
                    break;
                case Lifetime.TryAddSingleton:
                    services.TryAddSingleton(serviceType, type);
                    break;
                case Lifetime.Transient:
                    services.AddTransient(serviceType, type);
                    break;
                case Lifetime.TryAddTransient:
                    services.TryAddTransient(serviceType, type);
                    break;
            }
        }
        else
        {
            throw new InvalidOperationException($"Type {type.FullName} does not implement {serviceType.FullName}");
        }
    }
}