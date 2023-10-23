namespace Bindicate.Attributes.Options;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterOptionsAttribute : Attribute
{
    public string ConfigurationSection { get; }

    public RegisterOptionsAttribute(string configurationSection)
    {
        ConfigurationSection = configurationSection;
    }
}