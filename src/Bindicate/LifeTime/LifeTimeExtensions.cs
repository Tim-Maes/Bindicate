using Microsoft.Extensions.DependencyInjection;

namespace Bindicate;

public static class LifetimeExtensions
{
    public static ServiceLifetime ConvertToServiceLifetime(this Lifetime lifetime)
    {
        return lifetime switch
        {
            Lifetime.Scoped => ServiceLifetime.Scoped,
            Lifetime.TryAddScoped => ServiceLifetime.Scoped,
            Lifetime.Transient => ServiceLifetime.Transient,
            Lifetime.TryAddTransient => ServiceLifetime.Transient,
            Lifetime.Singleton => ServiceLifetime.Singleton,
            Lifetime.TryAddSingleton => ServiceLifetime.Singleton,
            _ => throw new System.ArgumentOutOfRangeException(nameof(lifetime), "Unsupported lifetime.")
        };
    }
}