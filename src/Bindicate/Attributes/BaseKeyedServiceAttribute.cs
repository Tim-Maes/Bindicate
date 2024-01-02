namespace Bindicate.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public abstract class BaseKeyedServiceAttribute : Attribute
{
    public Type ServiceType { get; protected set; }
    public object Key { get; protected set; }  // Added key property

    public abstract Lifetime.Lifetime Lifetime { get; }

    // Constructor for class-only registration without a key
    protected BaseKeyedServiceAttribute()
    {
        ServiceType = null;
    }

    // Constructor for explicit interface registration with a key
    protected BaseKeyedServiceAttribute(object key, Type serviceType = null)
    {
        Key = key;
        ServiceType = serviceType;
    }
}