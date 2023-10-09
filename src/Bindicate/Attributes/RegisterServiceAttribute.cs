namespace Bindicate.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterServiceAttribute : Attribute
{
    public Type ServiceType { get; }
    public Lifetime Lifetime { get; }

    // Constructor for class-only registration
    public RegisterServiceAttribute(Lifetime lifetime)
    {
        Lifetime = lifetime;
        ServiceType = null;
    }

    // Constructor for explicit interface registration
    public RegisterServiceAttribute(Type serviceType, Lifetime lifetime)
    {
        ServiceType = serviceType;
        Lifetime = lifetime;
    }
}