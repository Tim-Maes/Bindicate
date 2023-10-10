namespace Bindicate.Attributes;

public class AddScopedAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime => Lifetime.Scoped;

    public AddScopedAttribute() : base() { }

    public AddScopedAttribute(Type serviceType) : base(serviceType) { }
}