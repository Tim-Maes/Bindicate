using Bindicate.Attributes;
using Bindicate.Configuration;
using Bindicate.Tests.ScopedTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Xunit;

namespace Bindicate.Tests.Singleton;

public class AddSingletonAttributeTests
{
    private readonly Assembly _testAssembly = typeof(AddScopedAttributeTests).Assembly;

    [Fact]
    public void AddSingleton_AlwaysReturnsSame()
    {
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();

        var instance1 = serviceProvider.GetService<ISingletonInterface>();
        var instance2 = serviceProvider.GetService<ISingletonInterface>();

        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void AddSingleton_WithInterface_RegistersCorrectly()
    {
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<ISingletonInterface>();

            service.Should().NotBeNull().And.BeOfType<SingletonWithInterface>();
        }
    }

    [Fact]
    public void AddSingleton_RegistersCorrectly()
    {
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<SimpleSingletonClass>();

            service.Should().NotBeNull().And.BeOfType<SimpleSingletonClass>();
        }
    }
}

[AddSingleton]
public class SimpleSingletonClass { }

public interface ISingletonInterface { }

[AddSingleton(typeof(ISingletonInterface))]
public class SingletonWithInterface : ISingletonInterface { }

