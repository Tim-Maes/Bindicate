using Bindicate.Attributes;
using Bindicate.Configuration;
using Bindicate.Tests.ScopedTests;
using Microsoft.Extensions.DependencyInjection;

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
        // Arrange
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();

        // Act
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<ISingletonInterface>();
        ServiceDescriptor serviceDescriptor = services.First(x => x.ServiceType == typeof(ISingletonInterface));

        // Assert
        service.Should().NotBeNull().And.BeOfType<SingletonWithInterface>();
        serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddSingleton_RegistersCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();
        
        // Act
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<SimpleSingletonClass>();
        ServiceDescriptor serviceDescriptor = services.First(x => x.ServiceType == typeof(SimpleSingletonClass));

        // Assert
        service.Should().NotBeNull().And.BeOfType<SimpleSingletonClass>();
        serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }
}

[AddSingleton]
public class SimpleSingletonClass { }

public interface ISingletonInterface { }

[AddSingleton(typeof(ISingletonInterface))]
public class SingletonWithInterface : ISingletonInterface { }

