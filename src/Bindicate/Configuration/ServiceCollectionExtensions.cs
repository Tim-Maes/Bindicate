using Bindicate.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bindicate.Configuration;

public static class ServiceCollectionExtensions
{
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
                            RegisterService(services, serviceType, type, (s, t) => services.AddScoped(s, t));
                            break;
                        case Lifetime.Singleton:
                            RegisterService(services, serviceType, type, (s, t) => services.AddSingleton(s, t));
                            break;
                        case Lifetime.Transient:
                            RegisterService(services, serviceType, type, (s, t) => services.AddTransient(s, t));
                            break;
                        default:
                            throw new ArgumentException($"Unsupported lifetime: {attr.Lifetime}");
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

    private static void RegisterService(IServiceCollection services, Type serviceType, Type implementationType, Action<Type, Type?> registrationMethod)
    {
        if (serviceType == implementationType)
        {
            registrationMethod(implementationType, implementationType); 
        }
        else
        {
            registrationMethod(serviceType, implementationType);
        }
    }
}