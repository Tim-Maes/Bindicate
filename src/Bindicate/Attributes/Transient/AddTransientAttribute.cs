namespace Bindicate.Attributes;

public class AddTransientAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime => Lifetime.Transient;

    public AddTransientAttribute() : base() { }

    public AddTransientAttribute(Type serviceType) : base(serviceType) { }
}
