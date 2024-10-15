namespace Bindicate;

public enum Lifetime
{
    Scoped,
    TryAddScoped,
    Transient,
    TryAddTransient,
    Singleton,
    TryAddSingleton,
    TryAddEnumerableTransient,
    TryAddEnumerableScoped,
    TryAddEnumerableSingleton
}
