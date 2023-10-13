using Bindicate.LifeTime;

namespace Bindicate.Attributes;

/// <summary>
/// Attempts to register a service with the dependency injection container with a transient lifetime.
/// If a service with the same type is already registered, the existing registration is preserved.
/// </summary>
public class TryAddTransientAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime => Lifetime.TryAddTransient;

    public TryAddTransientAttribute() : base() { }

    public TryAddTransientAttribute(Type serviceType) : base(serviceType) { }
}
