namespace Bindicate.Configuration;

public class TypeMetadata
{
    public Type Type { get; }
    public bool HasRegisterOptionsAttribute { get; }
    public bool HasBaseServiceAttribute { get; }
    public bool HasBaseKeyedServiceAttribute { get; }

    public TypeMetadata(Type type, bool hasRegisterOptionsAttribute, bool hasBaseServiceAttribute, bool hasBaseKeyedServiceAttribute)
    {
        Type = type;
        HasRegisterOptionsAttribute = hasRegisterOptionsAttribute;
        HasBaseServiceAttribute = hasBaseServiceAttribute;
        HasBaseKeyedServiceAttribute = hasBaseKeyedServiceAttribute;
    }
}