using Bindicate.Lifetime;

namespace Bindicate.Attributes;

/// <summary>
/// Attempts to register a service with the dependency injection container with a singleton lifetime.
/// If a service with the same type is already registered, the existing registration is preserved.
/// </summary>
public class TryAddSingletonAttribute : BaseServiceAttribute
{
    public override Lifetime.Lifetime Lifetime => Bindicate.Lifetime.Lifetime.TryAddSingleton;

    public TryAddSingletonAttribute() : base() { }

    public TryAddSingletonAttribute(Type serviceType) : base(serviceType) { }
}
