using Bindicate.Attributes;
using Bindicate.Configuration;
using Bindicate.Tests.ScopedTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Xunit;

namespace Bindicate.Tests.Transient;

public class AddTransientAttributeTests
{
    private readonly Assembly _testAssembly = typeof(AddScopedAttributeTests).Assembly;

    [Fact]
    public void AddTransient_AlwaysReturnsNewInstance()
    {
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();

        var instance1 = serviceProvider.GetService<ITransientInterface>();
        var instance2 = serviceProvider.GetService<ITransientInterface>();

        instance1.Should().NotBeSameAs(instance2);
    }

    [Fact]
    public void AddTransient_WithInterface_RegistersCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();
        
        // Act
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<ITransientInterface>();
        ServiceDescriptor serviceDescriptor = services.First(x => x.ServiceType == typeof(ITransientInterface));

        // Assert
        service.Should().NotBeNull().And.BeOfType<TransientWithInterface>();
        serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
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
        var service = scope.ServiceProvider.GetService<SimpleTransientClass>();
        ServiceDescriptor serviceDescriptor = services.First(x => x.ServiceType == typeof(SimpleTransientClass));

        // Assert
        service.Should().NotBeNull().And.BeOfType<SimpleTransientClass>();
        serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
    }
}

[AddTransient]
public class SimpleTransientClass { }

public interface ITransientInterface { }

[AddTransient(typeof(ITransientInterface))]
public class TransientWithInterface : ITransientInterface { }