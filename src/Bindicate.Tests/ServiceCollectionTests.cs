using Bindicate.Attributes;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Bindicate.Configuration;

namespace Bindicate.Tests;

public class ServiceCollectionTests
{
    [Fact]
    public void AddBindicate_RegistersServiceCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var testAssembly = typeof(ServiceCollectionTests).Assembly;

        // Act
        services.AddBindicate(testAssembly);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var serviceInstance = serviceProvider.GetService<ITestService>();

        serviceInstance.Should().NotBeNull().And.BeOfType<TestService>();
    }

    [Fact]
    public void AddBindicate_RegistersClassCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var testAssembly = typeof(ServiceCollectionTests).Assembly;

        // Act
        services.AddBindicate(testAssembly);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var serviceInstance = serviceProvider.GetService<TestClass>();

        serviceInstance.Should().NotBeNull().And.BeOfType<TestClass>();
    }
}

public interface ITestService { }

[RegisterService(typeof(ITestService), Lifetime.Transient)]
public class TestService : ITestService { }

[RegisterService(Lifetime.Scoped)]
public class TestClass : ITestService { }