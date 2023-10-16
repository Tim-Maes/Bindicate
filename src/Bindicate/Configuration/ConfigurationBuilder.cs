using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bindicate.Configuration;

public class BindicateConfigurationBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<Assembly> _assemblies = new();

    public BindicateConfigurationBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public BindicateConfigurationBuilder WithAssemblies(params Assembly[] assemblies)
    {
        _assemblies.AddRange(assemblies);

        return this;
    }

    public void Register()
    {
        foreach (var assembly in _assemblies)
        {
            _services.AddBindicate(assembly);
        }
    }
}