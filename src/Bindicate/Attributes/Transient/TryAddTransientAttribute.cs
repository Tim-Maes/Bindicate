namespace Bindicate.Attributes;

public class TryAddTransientAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime => Lifetime.TryAddTransient;

    public TryAddTransientAttribute() : base() { }

    public TryAddTransientAttribute(Type serviceType) : base(serviceType) { }
}
