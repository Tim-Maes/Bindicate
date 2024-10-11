namespace Bindicate.Attributes;

/// <summary>
/// Specifies that a class is a decorator for a specified service type.
/// Decorators are applied in the order specified by the <see cref="Order"/> property.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RegisterDecoratorAttribute : Attribute
{
    /// <summary>
    /// Gets the type of the service to be decorated.
    /// </summary>
    public Type ServiceType { get; }

    /// <summary>
    /// Gets the order in which the decorator should be applied.
    /// Lower values are applied first.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterDecoratorAttribute"/> class.
    /// </summary>
    /// <param name="serviceType">The type of the service to be decorated.</param>
    /// <param name="order">The order in which the decorator should be applied. Lower values are applied first.</param>
    public RegisterDecoratorAttribute(Type serviceType, int order = 0)
    {
        ServiceType = serviceType;
        Order = order;
    }
}
