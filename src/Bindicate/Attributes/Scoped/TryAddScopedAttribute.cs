using Bindicate.Lifetime;

namespace Bindicate.Attributes;

/// <summary>
/// Specifies that a service should be registered with the dependency injection container for the lifetime of a scope.
/// Unlike the <see cref="AddScopedAttribute"/>, if the service is already registered with the container,
/// this attribute doesn't replace the existing registration.
/// </summary>
public class TryAddScopedAttribute : BaseServiceAttribute
{
    public override Lifetime.Lifetime Lifetime => Bindicate.Lifetime.Lifetime.TryAddScoped;

    public TryAddScopedAttribute() : base() { }

    public TryAddScopedAttribute(Type serviceType) : base(serviceType) { }
}