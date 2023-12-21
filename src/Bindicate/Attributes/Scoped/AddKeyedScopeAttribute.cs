namespace Bindicate.Attributes.Scoped;

/// <summary>
/// Specifies that a service should be registered with the dependency injection container with a specified key.
/// </summary>
public class AddKeyedScopedAttribute : BaseServiceAttribute
{
    public object Key { get; private set; }

    public AddKeyedScopedAttribute(object key, Type serviceType = null)
        : base(serviceType)
    {
        Key = key;
    }

    public override Lifetime.Lifetime Lifetime => Bindicate.Lifetime.Lifetime.Scoped;
}