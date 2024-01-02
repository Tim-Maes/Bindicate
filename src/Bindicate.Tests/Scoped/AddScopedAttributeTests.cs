using Bindicate.Attributes;
using Bindicate.Attributes.Scoped;
using Microsoft.Extensions.DependencyInjection;

namespace Bindicate.Tests.ScopedTests;

public class AddScopedAttributeTests
{
    private readonly Assembly _testAssembly = typeof(AddScopedAttributeTests).Assembly;

    [Fact]
    public void AddScoped_WithoutAttribute_NotRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly).Register();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<NonRegisteredClass>();

        // Assert
        service.Should().BeNull();
    }

    [Fact]
    public void AddScoped_WithInterface_RegistersCorrectly()
    {
        //Arrange
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly).Register();
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
        services.AddAutowiringForAssembly(_testAssembly).Register();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<SimpleScopedClass>();
        ServiceDescriptor serviceDescriptor = services.First(x => x.ServiceType == typeof(SimpleScopedClass));

        // Assert
        service.Should().NotBeNull().And.BeOfType<SimpleScopedClass>();
        serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }


    [Fact]
    public void AddKeyedScoped_WithMultipleKeys_RegistersCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly).ForKeyedServices().Register();
        var serviceProvider = services.BuildServiceProvider();

        // Act and Assert for the first key
        using var scope1 = serviceProvider.CreateScope();
        var service1 = scope1.ServiceProvider.GetKeyedService<IKeyedService>("myKey");
        service1.Should().NotBeNull().And.BeOfType<KeyedService>();

        // Act and Assert for the second key
        using var scope2 = serviceProvider.CreateScope();
        var service2 = scope2.ServiceProvider.GetKeyedService<IKeyedService>("mySecondKey");
        service2.Should().NotBeNull().And.BeOfType<SecondKeyedService>();
    }
}

public class NonRegisteredClass { }

[AddScoped]
public class SimpleScopedClass { }

public interface IScopedInterface { }

[AddScoped(typeof(IScopedInterface))]
public class ScopedWithInterface : IScopedInterface { }

[AddKeyedScoped("myKey", typeof(IKeyedService))]
public class KeyedService : IKeyedService { }

[AddKeyedScoped("mySecondKey", typeof(IKeyedService))]
public class SecondKeyedService : IKeyedService { }

public interface IKeyedService { }
