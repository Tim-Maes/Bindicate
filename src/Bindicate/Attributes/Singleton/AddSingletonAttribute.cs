namespace Bindicate.Attributes;

/// <summary>
/// Specifies that a service should be registered with the dependency injection container with a singleton lifetime.
/// This means a single instance is created and it acts as a singleton.
/// </summary>
public class AddSingletonAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime => Lifetime.Singleton;

    public AddSingletonAttribute() : base() { }

    public AddSingletonAttribute(Type serviceType) : base(serviceType) { }
}
