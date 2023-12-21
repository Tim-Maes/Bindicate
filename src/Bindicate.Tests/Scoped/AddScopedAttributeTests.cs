using Bindicate.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Bindicate.Tests.ScopedTests;

public class AddScopedAttributeTests
{
    private readonly Assembly _testAssembly = typeof(AddScopedAttributeTests).Assembly;

    [Fact]
    public void AddScoped_WithInterface_RegistersCorrectly()
    {
        //Arrange
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();

        //Act
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<IScopedInterface>();
        ServiceDescriptor serviceDescriptor = services.First(x => x.ServiceType == typeof(IScopedInterface));

        // Assert        
        service.Should().NotBeNull().And.BeOfType<ScopedWithInterface>();
        serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddScoped_RegistersCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();

        // Act
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<SimpleScopedClass>();
        ServiceDescriptor serviceDescriptor = services.First(x => x.ServiceType == typeof(SimpleScopedClass));

        // Assert
        service.Should().NotBeNull().And.BeOfType<SimpleScopedClass>();
        serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }
}

[AddScoped]
public class SimpleScopedClass { }

public interface IScopedInterface { }

[AddScoped(typeof(IScopedInterface))]
public class ScopedWithInterface : IScopedInterface { }
