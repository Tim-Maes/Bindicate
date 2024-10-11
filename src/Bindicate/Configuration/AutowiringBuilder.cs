using Bindicate.Attributes;
using Bindicate.Attributes.Options;
using Bindicate.Attributes.Scoped;
using Bindicate.Attributes.Singleton;
using Bindicate.Attributes.Transient;
using Bindicate.Configuration;
using Bindicate.Lifetime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Bindicate;

public class AutowiringBuilder
{
    private IServiceCollection _services { get; }
    private readonly List<TypeMetadata> _typeMetadatas;


    public AutowiringBuilder(IServiceCollection services, Assembly targetAssembly)
    {
        _services = services;
        _typeMetadatas = ScanAssembly(targetAssembly);

        AddAutowiringForAssembly();
    }

    private List<TypeMetadata> ScanAssembly(Assembly assembly)
    {
        var typeMetadatas = new List<TypeMetadata>();

        foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract))
        {
            var hasRegisterOptionsAttribute = type.IsDefined(typeof(RegisterOptionsAttribute), false);
            var hasBaseServiceAttribute = type.IsDefined(typeof(BaseServiceAttribute), false);
            var hasBaseKeyedServiceAttribute = type.IsDefined(typeof(BaseKeyedServiceAttribute), false);
            var hasDecoratorAttribute = type.IsDefined(typeof(RegisterDecoratorAttribute), false);

            var typeMetadata = new TypeMetadata(type, hasRegisterOptionsAttribute, hasBaseServiceAttribute, hasBaseKeyedServiceAttribute, hasDecoratorAttribute);
            typeMetadatas.Add(typeMetadata);
        }

        return typeMetadatas;
    }


    /// <summary>
    /// Scans the assembly to automatically wire up services based on the attributes.
    /// </summary>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AutowiringBuilder AddAutowiringForAssembly()
    {
        foreach (var typeMetadata in _typeMetadatas)
        {
            if (typeMetadata.HasBaseServiceAttribute)
            {
                var type = typeMetadata.Type;
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
        foreach (var typeMetadata in _typeMetadatas)
        {
            if (typeMetadata.HasRegisterOptionsAttribute)
            {
                var type = typeMetadata.Type;
                var optionAttributes = type.GetCustomAttributes(typeof(RegisterOptionsAttribute), false)
                                       .Cast<RegisterOptionsAttribute>();

                foreach (var attr in optionAttributes)
                {
                    var configSection = configuration.GetSection(attr.ConfigurationSection);

                    if (!configSection.Exists())
                        throw new InvalidOperationException($"Missing configuration section: {attr.ConfigurationSection}");

                    var genericOptionsConfigureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
                            .GetMethods()
                            .FirstOrDefault(m => m.Name == "Configure" && m.GetParameters().Length == 2);

                    var specializedMethod = genericOptionsConfigureMethod.MakeGenericMethod(type);
                    specializedMethod.Invoke(null, new object[] { _services, configSection });
                }
            }
        }

        return this;
    }

    /// <summary>
    /// Scans the assembly to automatically wire up keyed services based on the attributes.
    /// </summary>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AutowiringBuilder ForKeyedServices()
    {
        foreach (var typeMetadata in _typeMetadatas)
        {
            if (typeMetadata.HasBaseKeyedServiceAttribute)
            {
                var type = typeMetadata.Type;

                var keyedAttributes = type.GetCustomAttributes(typeof(BaseKeyedServiceAttribute), false)
                                      .Cast<BaseKeyedServiceAttribute>();

                foreach (var attr in keyedAttributes)
                {
                    var serviceType = attr.ServiceType ?? type;
                    var key = attr.Key;

                    var registrationMethod = GetKeyedRegistrationMethod(_services, attr);

                    registrationMethod(serviceType, key, type);
                }
            }
        }

        return this;
    }

    /// <summary>
    /// Scans the assembly to automatically wire up decorators based on the attributes.
    /// </summary>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public AutowiringBuilder AddDecorators()
    {
        var decoratorsByService = _typeMetadatas
            .Where(tm => tm.HasDecoratorAttribute)
            .SelectMany(tm => tm.Type.GetCustomAttributes<RegisterDecoratorAttribute>()
                .Select(attr => new
                {
                    ServiceType = attr.ServiceType,
                    Order = attr.Order,
                    DecoratorType = tm.Type
                }))
            .GroupBy(x => x.ServiceType)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(x => x.Order).Select(x => x.DecoratorType).ToList()
            );

        foreach (var kvp in decoratorsByService)
        {
            var serviceType = kvp.Key;
            var decoratorTypes = kvp.Value;

            var originalDescriptorIndex = _services
                .Select((sd, index) => new { sd, index })
                .FirstOrDefault(sd => sd.sd.ServiceType == serviceType);

            if (originalDescriptorIndex == null)
            {
                throw new InvalidOperationException($"Service type {serviceType.FullName} is not registered.");
            }

            _services.RemoveAt(originalDescriptorIndex.index);

            ServiceDescriptor currentDescriptor = originalDescriptorIndex.sd;

            foreach (var decoratorType in decoratorTypes)
            {
                currentDescriptor = CreateDecoratorDescriptor(serviceType, decoratorType, currentDescriptor);
            }

            _services.Insert(originalDescriptorIndex.index, currentDescriptor);
        }

        return this;
    }

    private ServiceDescriptor CreateDecoratorDescriptor(Type serviceType, Type decoratorType, ServiceDescriptor previousDescriptor)
    {
        Func<IServiceProvider, object> previousImplementationFactory;

        if (previousDescriptor.ImplementationFactory != null)
        {
            previousImplementationFactory = previousDescriptor.ImplementationFactory;
        }
        else if (previousDescriptor.ImplementationInstance != null)
        {
            var instance = previousDescriptor.ImplementationInstance;
            previousImplementationFactory = provider => instance;
        }
        else if (previousDescriptor.ImplementationType != null)
        {
            var implementationType = previousDescriptor.ImplementationType;
            previousImplementationFactory = provider => ActivatorUtilities.CreateInstance(provider, implementationType);
        }
        else
        {
            throw new InvalidOperationException("Unsupported service descriptor.");
        }

        return ServiceDescriptor.Describe(
            serviceType,
            provider =>
            {
                var decoratorConstructor = decoratorType.GetConstructors().First();
                var parameters = decoratorConstructor.GetParameters();

                var args = parameters.Select(p =>
                {
                    if (p.ParameterType == serviceType)
                    {
                        return previousImplementationFactory(provider);
                    }
                    else
                    {
                        return provider.GetService(p.ParameterType);
                    }
                }).ToArray();

                return Activator.CreateInstance(decoratorType, args);
            },
            previousDescriptor.Lifetime
        );
    }

    /// <summary>
    /// Registers all configured services and options into the IServiceCollection.
    /// </summary>
    /// <returns>The IServiceCollection that services and options were registered into.</returns>
    public IServiceCollection Register()
    {
        AddDecorators();
        return _services;
    }
    private static Action<Type, object, Type> GetKeyedRegistrationMethod(IServiceCollection services, BaseKeyedServiceAttribute attr)
    => attr switch
    {
        AddKeyedScopedAttribute _ => (s, k, t) => services.AddKeyedScoped(s, k, t),
        AddKeyedSingletonAttribute _ => (s, k, t) => services.AddKeyedSingleton(s, k, t),
        AddKeyedTransientAttribute _ => (s, k, t) => services.AddKeyedTransient(s, k, t),
        _ => throw new ArgumentOutOfRangeException(nameof(attr), "Unsupported attribute type.")
    };


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
