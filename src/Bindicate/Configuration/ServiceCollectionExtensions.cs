using Bindicate.Attributes;
using Bindicate.Lifetime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Bindicate.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static BindicateConfigurationBuilder AddBindicate(this IServiceCollection services)
        {
            return new BindicateConfigurationBuilder(services);
        }

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

                    if (serviceType.IsDefined(typeof(RegisterGenericInterfaceAttribute), false))
                    {
                        if (serviceType.IsGenericType)
                        {
                            services.Add(ServiceDescriptor.Describe(
                                serviceType.GetGenericTypeDefinition(),
                                type.GetGenericTypeDefinition(),
                                attr.Lifetime.ConvertToServiceLifetime())
                            );
                        }
                        else
                        {
                            // Handle non-generic services with generic interfaces
                            foreach (var iface in type.GetInterfaces())
                            {
                                if (iface.IsGenericType && iface.GetGenericTypeDefinition().IsDefined(typeof(RegisterGenericInterfaceAttribute), false))
                                {
                                    var genericInterface = iface.GetGenericTypeDefinition();
                                    services.Add(ServiceDescriptor.Describe(genericInterface, type, attr.Lifetime.ConvertToServiceLifetime()));
                                }
                            }
                        }
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

        private static Action<Type, Type> GetRegistrationMethod(IServiceCollection services, Lifetime.Lifetime lifetime)
            => lifetime switch
            {
                Lifetime.Lifetime.Scoped => (s, t) => services.AddScoped(s, t),
                Lifetime.Lifetime.Singleton => (s, t) => services.AddSingleton(s, t),
                Lifetime.Lifetime.Transient => (s, t) => services.AddTransient(s, t),
                Lifetime.Lifetime.TryAddScoped => (s, t) => services.TryAddScoped(s, t),
                Lifetime.Lifetime.TryAddSingleton => (s, t) => services.TryAddSingleton(s, t),
                Lifetime.Lifetime.TryAddTransient => (s, t) => services.TryAddTransient(s, t),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), "Unsupported lifetime.")
            };

        private static void RegisterService(Type serviceType, Type implementationType, Action<Type, Type> registrationMethod)
        {
            registrationMethod(serviceType, implementationType);
        }
    }
}
