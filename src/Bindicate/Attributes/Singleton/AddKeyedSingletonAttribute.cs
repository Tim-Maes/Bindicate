namespace Bindicate.Attributes.Singleton;

/// <summary>
/// Specifies that a service should be registered with the dependency injection container with a specified key.
/// </summary>
public class AddKeyedSingletonAttribute : BaseKeyedServiceAttribute
{
    public AddKeyedSingletonAttribute(object key, Type serviceType)
        : base(key, serviceType)
    {
    }

    public override Lifetime.Lifetime Lifetime => Bindicate.Lifetime.Lifetime.Singleton;
}