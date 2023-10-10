namespace Bindicate.Attributes;

public class TryAddScopedAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime => Lifetime.TryAddScoped;

    public TryAddScopedAttribute() : base() { }

    public TryAddScopedAttribute(Type serviceType) : base(serviceType) { }
}