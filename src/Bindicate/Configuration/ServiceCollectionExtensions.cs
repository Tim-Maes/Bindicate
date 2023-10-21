using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bindicate.Configuration;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutowiringBuilder"/> class for automatically
    /// registering services from the specified assembly.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="targetAssembly">The assembly to scan for types to register.</param>
    /// <returns>An instance of <see cref="AutowiringBuilder"/> configured with the provided services and assembly.</returns>
    public static AutowiringBuilder AddAutowiringForAssembly(this IServiceCollection services, Assembly targetAssembly)
        => new AutowiringBuilder(services, targetAssembly);
}
