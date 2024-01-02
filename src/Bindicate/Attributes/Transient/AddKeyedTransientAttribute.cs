namespace Bindicate.Attributes.Transient;

/// <summary>
/// Specifies that a service should be registered with the dependency injection container with a specified key.
/// </summary>
public class AddKeyedTransientAttribute : BaseKeyedServiceAttribute
{
    public AddKeyedTransientAttribute(object key, Type serviceType)
        : base(key, serviceType)
    {
    }

    public override Lifetime.Lifetime Lifetime => Bindicate.Lifetime.Lifetime.Transient;
}