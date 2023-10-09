using Bindicate.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bindicate.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddBindicate(this IServiceCollection services, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
        {
            var registerAttributes = type.GetCustomAttributes(typeof(RegisterServiceAttribute), false)
                                         .Cast<RegisterServiceAttribute>();

            foreach (var attr in registerAttributes)
            {
                if (type.GetInterfaces().Contains(attr.ServiceType) || type == attr.ServiceType)
                {
                    switch (attr.Lifetime)
                    {
                        case Lifetime.Scoped:
                            services.AddScoped(attr.ServiceType, type);
                            break;
                        case Lifetime.Singleton:
                            services.AddSingleton(attr.ServiceType, type);
                            break;
                        case Lifetime.Transient:
                            services.AddTransient(attr.ServiceType, type);
                            break;
                        default:
                            throw new ArgumentException($"Unsupported lifetime: {attr.Lifetime}");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Type {type.FullName} does not implement {attr.ServiceType.FullName}");
                }
            }
        }
    }
}