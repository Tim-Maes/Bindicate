using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bindicate.Configuration;

public static class ServiceCollectionExtensions
{
    public static AutowiringBuilder AddAutowiringForAssembly(this IServiceCollection services, Assembly targetAssembly)
        => new AutowiringBuilder(services, targetAssembly);
}
