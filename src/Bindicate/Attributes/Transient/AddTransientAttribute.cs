namespace Bindicate.Attributes;

/// <summary>
/// Specifies that a service should be registered with the dependency injection container with a transient lifetime.
/// This means a new instance is created every time it is requested.
/// </summary>
public class AddTransientAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime => Bindicate.Lifetime.Transient;

    public AddTransientAttribute() : base() { }

    public AddTransientAttribute(Type serviceType) : base(serviceType) { }
}
