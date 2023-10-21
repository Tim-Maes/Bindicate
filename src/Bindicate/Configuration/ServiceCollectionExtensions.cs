using Bindicate.Attributes;
using Bindicate.Attributes.Options;
using Bindicate.Lifetime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;

namespace Bindicate.Configuration;

public static class ServiceCollectionExtensions
{
    public static AutowiringBuilder AddAutowiringForAssembly(this IServiceCollection services, Assembly targetAssembly)
    {
        return new AutowiringBuilder(services, targetAssembly);
    }
}
