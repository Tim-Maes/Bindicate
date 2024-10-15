namespace Bindicate.Attributes.Enumerable;

/// <summary>
/// Specifies that a service should be registered using TryAddEnumerable with the dependency injection container.
/// This allows multiple implementations of the same service type to be registered.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class TryAddEnumerableAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime { get; }

    public TryAddEnumerableAttribute(Lifetime lifetime)
    {
        Lifetime = lifetime;
    }

    public TryAddEnumerableAttribute(Lifetime lifetime, Type serviceType) : base(serviceType)
    {
        Lifetime = lifetime;
    }
}