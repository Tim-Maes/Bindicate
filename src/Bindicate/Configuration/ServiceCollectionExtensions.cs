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
                var registrationMethod = GetRegistrationMethod(services, attr.Lifetime);

                if (serviceType.IsDefined(typeof(AddGenericInterfaceAttribute), false))
                {
                    RegisterService(serviceType, type, registrationMethod);
                }
                else if (type.GetInterfaces().Contains(serviceType) || type == serviceType)
                {
                    RegisterService(serviceType, type, registrationMethod);
                }
                else
                {
                    throw new InvalidOperationException($"Type {type.FullName} does not implement {serviceType.FullName}");
                }
            }
        }

        return services;
    }

    private static Action<Type, Type> GetRegistrationMethod(IServiceCollection services, Lifetime lifetime)
    => lifetime switch
    {
        Lifetime.Scoped => (s, t) => services.AddScoped(s, t),
        Lifetime.Singleton => (s, t) => services.AddSingleton(s, t),
        Lifetime.Transient => (s, t) => services.AddTransient(s, t),
        Lifetime.TryAddScoped => (s, t) => services.TryAddScoped(s, t),
        Lifetime.TryAddSingleton => (s, t) => services.TryAddSingleton(s, t),
        Lifetime.TryAddTransient => (s, t) => services.TryAddTransient(s, t),
    };

    private static void RegisterService(Type serviceType, Type implementationType, Action<Type, Type> registrationMethod)
    {
        registrationMethod(serviceType, implementationType);
    }
}