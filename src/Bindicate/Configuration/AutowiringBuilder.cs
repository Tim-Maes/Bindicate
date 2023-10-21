using Bindicate.Attributes;
using Bindicate.Attributes.Options;
using Bindicate.Lifetime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Bindicate.Configuration;

public class AutowiringBuilder
{
    public IServiceCollection _services { get; }

    public Assembly _targetAssembly { get; }

    public AutowiringBuilder(IServiceCollection services, Assembly targetAssembly)
    {
        _services = services;
        _targetAssembly = targetAssembly;
        AddAutowiringForAssembly();
    }

    /// <summary>
    /// Scans the assembly to automatically wire up services based on the attributes.
    /// </summary>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AutowiringBuilder AddAutowiringForAssembly()
    {
        foreach (var type in _targetAssembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
        {
            var registerAttributes = type.GetCustomAttributes(typeof(BaseServiceAttribute), false)
                                         .Cast<BaseServiceAttribute>();

            foreach (var attr in registerAttributes)
            {
                var serviceType = attr.ServiceType ?? type;
                var registrationMethod = GetRegistrationMethod(_services, attr.Lifetime);

                if (serviceType.IsDefined(typeof(RegisterGenericInterfaceAttribute), false))
                {
                    if (serviceType.IsGenericType)
                    {
                        _services.Add(ServiceDescriptor.Describe(
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
                                _services.Add(ServiceDescriptor.Describe(genericInterface, type, attr.Lifetime.ConvertToServiceLifetime()));
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

        return this;
    }

    /// <summary>
    /// Scans assemblies to find classes annotated with RegisterOptionsAttribute,
    /// and configures them as options from the provided IConfiguration object.
    /// </summary>
    /// <param name="configuration">The IConfiguration object to read the settings from.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AutowiringBuilder WithOptions(IConfiguration configuration)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
            {
                var optionAttributes = type.GetCustomAttributes(typeof(RegisterOptionsAttribute), false)
                                           .Cast<RegisterOptionsAttribute>();

                foreach (var attr in optionAttributes)
                {
                    //TODO: remove these exceptions
                    if (attr == null) throw new InvalidOperationException("attr is null");
                    if (configuration == null) throw new InvalidOperationException("configuration is null");

                    var configSection = configuration.GetSection(attr.ConfigurationSection);

                    var genericMethod = typeof(OptionsConfigurationServiceCollectionExtensions).GetMethod("Configure", new[] { typeof(IConfiguration) });
                    if (genericMethod == null) throw new InvalidOperationException("genericMethod is null");

                    var specializedMethod = genericMethod.MakeGenericMethod(type);
                    if (specializedMethod == null) throw new InvalidOperationException("specializedMethod is null");

                    specializedMethod.Invoke(null, new object[] { _services, configSection });

                    // Add PostConfigure to perform validation
                    var postConfigureMethod = typeof(OptionsServiceCollectionExtensions)
                        .GetMethods()
                        .Where(m => m.Name == "PostConfigure" && m.GetParameters().Length == 2)
                        .First();
                    var postConfigureSpecialized = postConfigureMethod.MakeGenericMethod(type);

                    postConfigureSpecialized.Invoke(null, new object[]
                    {
                        _services,
                        new Action<object>(options =>
                        {
                            foreach (var prop in type.GetProperties())
                            {
                                var value = prop.GetValue(options);
                                if (value == null || (prop.PropertyType.IsValueType && value.Equals(Activator.CreateInstance(prop.PropertyType))))
                                {
                                    throw new InvalidOperationException($"Missing configuration for property {prop.Name} in section {attr.ConfigurationSection}");
                                }
                            }
                        })
                    });
                }
            }
        }

        return this;
    }

    /// <summary>
    /// Registers all configured services and options into the IServiceCollection.
    /// </summary>
    /// <returns>The IServiceCollection that services and options were registered into.</returns>
    public IServiceCollection Register()
    {
        return _services;
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
