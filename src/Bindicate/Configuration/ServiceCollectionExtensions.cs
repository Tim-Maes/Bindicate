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
            var registerAttributes = type.GetCustomAttributes(typeof(RegisterServiceAttribute), false)
                                         .Cast<RegisterServiceAttribute>();

            foreach (var attr in registerAttributes)
            {
                var serviceType = attr.ServiceType ?? type;

                if (type.GetInterfaces().Contains(serviceType) || type == serviceType)
                {
                    switch (attr.Lifetime)
                    {
                        case Lifetime.Scoped:
                            if (serviceType == type)
                                services.AddScoped(type);
                            else
                                services.AddScoped(serviceType, type);
                            break;
                        case Lifetime.Singleton:
                            if (serviceType == type)
                                services.AddSingleton(type);
                            else
                                services.AddSingleton(serviceType, type);
                            break;
                        case Lifetime.Transient:
                            if (serviceType == type)
                                services.AddTransient(type);
                            else
                                services.AddTransient(serviceType, type);
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
}