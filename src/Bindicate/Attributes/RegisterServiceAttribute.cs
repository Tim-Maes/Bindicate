namespace Bindicate.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegisterServiceAttribute : Attribute
{
    public Type ServiceType { get; }
    public Lifetime Lifetime { get; }

    public RegisterServiceAttribute(Type serviceType, Lifetime lifetime)
    {
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        Lifetime = lifetime;
    }
}
