namespace Bindicate.Attributes;

public class AddSingletonAttribute : BaseServiceAttribute
{
    public override Lifetime Lifetime => Lifetime.Singleton;

    public AddSingletonAttribute() : base() { }

    public AddSingletonAttribute(Type serviceType) : base(serviceType) { }
}
