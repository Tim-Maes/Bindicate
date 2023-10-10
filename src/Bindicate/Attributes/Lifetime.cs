namespace Bindicate.Attributes;

public enum Lifetime
{
    Scoped,
    TryAddScoped,
    Transient,
    TryAddTransient,
    Singleton,
    TryAddSingleton
}
