namespace Bindicate.Attributes;

public class TryAddSingletonAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime => Lifetime.TryAddSingleton;

    public TryAddSingletonAttribute() : base() { }

    public TryAddSingletonAttribute(Type serviceType) : base(serviceType) { }
}
