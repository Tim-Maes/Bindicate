namespace Bindicate.Attributes.Scoped;

/// <summary>
/// Specifies that a service should be registered with the dependency injection container with a specified key.
/// </summary>
public class AddKeyedScopedAttribute : BaseKeyedServiceAttribute
{
    public AddKeyedScopedAttribute(object key, Type serviceType)
        : base(key, serviceType)
    {
    }

    public override Lifetime.Lifetime Lifetime => Bindicate.Lifetime.Lifetime.Scoped;

}