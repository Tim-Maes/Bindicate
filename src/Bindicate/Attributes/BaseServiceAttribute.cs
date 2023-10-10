namespace Bindicate.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public abstract class BaseServiceAttribute : Attribute
{
    public Type ServiceType { get; protected set; }

    public abstract Lifetime Lifetime { get; }

    // Constructor for class-only registration
    protected BaseServiceAttribute()
    {
        ServiceType = null;
    }

    // Constructor for explicit interface registration
    protected BaseServiceAttribute(Type serviceType)
    {
        ServiceType = serviceType;
    }
}