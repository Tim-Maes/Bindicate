namespace Bindicate.Configuration;

public class TypeMetadata
{
    public Type Type { get; }
    public bool HasRegisterOptionsAttribute { get; }
    public bool HasBaseServiceAttribute { get; }
    public bool HasBaseKeyedServiceAttribute { get; }
    public bool HasDecoratorAttribute { get; }
    public bool HasTryAddEnumerableAttribute { get; }

    public TypeMetadata(Type type, bool hasRegisterOptionsAttribute, bool hasBaseServiceAttribute, bool hasBaseKeyedServiceAttribute, bool hasDecoratorAttribute, bool hasTryAddEnumerableAttribute)
    {
        Type = type;
        HasRegisterOptionsAttribute = hasRegisterOptionsAttribute;
        HasBaseServiceAttribute = hasBaseServiceAttribute;
        HasBaseKeyedServiceAttribute = hasBaseKeyedServiceAttribute;
        HasDecoratorAttribute = hasDecoratorAttribute;
        HasTryAddEnumerableAttribute = hasTryAddEnumerableAttribute;
    }
}