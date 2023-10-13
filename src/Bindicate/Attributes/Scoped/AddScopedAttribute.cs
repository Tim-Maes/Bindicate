using Bindicate.LifeTime;

namespace Bindicate.Attributes;

/// <summary>
/// Specifies that a service should be registered with the dependency injection container with a scoped lifetime.
/// This means a new instance is created once per scope.
/// </summary>
public class AddScopedAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime => Lifetime.Scoped;

    public AddScopedAttribute() : base() { }

    public AddScopedAttribute(Type serviceType) : base(serviceType) { }
}