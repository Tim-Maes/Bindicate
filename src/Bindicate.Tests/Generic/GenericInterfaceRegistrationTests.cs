using Bindicate.Attributes;
using Bindicate.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bindicate.Tests.Generic;

public class GenericInterfaceRegistrationTests
{
    [Fact]
    public void ShouldRegisterMultipleServices_WithGenericInterface1_AsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(ServiceWithGenericInterface1).Assembly;

        // Act
        services.AddBindicate(assembly);
        var serviceProvider = services.BuildServiceProvider();
        var instances = serviceProvider.GetServices<IGenericInterFace1>().ToList();
        var scope = serviceProvider.CreateScope();
        var scopedInstances = scope.ServiceProvider.GetServices<IGenericInterFace1>().ToList();

        // Assert
        instances.Should().NotBeNullOrEmpty();
        instances.Should().HaveCount(2);
        instances.Should().Contain(x => x.GetType() == typeof(ServiceWithGenericInterface1));
        instances.Should().Contain(x => x.GetType() == typeof(Service2WithGenericInterface1));

        scopedInstances.Should().NotBeNullOrEmpty();
        scopedInstances.Should().HaveCount(2);
        scopedInstances.Should().Contain(x => x.GetType() == typeof(ServiceWithGenericInterface1));
        scopedInstances.Should().Contain(x => x.GetType() == typeof(Service2WithGenericInterface1));
    }

    [Fact]
    public void ShouldRegisterMultipleServices_WithGenericInterface2_AsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(ServiceWithGenericInterface1).Assembly;

        // Act
        services.AddBindicate(assembly);
        var serviceProvider = services.BuildServiceProvider();
        var instances = serviceProvider.GetServices<IGenericInterFace2>().ToList();
        var scope = serviceProvider.CreateScope();
        var scopedInstances = scope.ServiceProvider.GetServices<IGenericInterFace2>().ToList();

        // Assert
        instances.Should().NotBeNullOrEmpty();
        instances.Should().HaveCount(2);
        instances.Should().Contain(x => x.GetType() == typeof(ServiceWithGenericInterface2));
        instances.Should().Contain(x => x.GetType() == typeof(Service2WithGenericInterface2));

        scopedInstances.Should().NotBeNullOrEmpty();
        scopedInstances.Should().HaveCount(2);
        scopedInstances.Should().Contain(x => x.GetType() == typeof(ServiceWithGenericInterface2));
        scopedInstances.Should().Contain(x => x.GetType() == typeof(Service2WithGenericInterface2));
    }

    [Fact]
    public void ShouldRegisterMultipleServices_WithGenericInterface1_AsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(ServiceWithGenericInterface1).Assembly;

        // Act
        services.AddBindicate(assembly);
        var serviceProvider = services.BuildServiceProvider();
        var instances = serviceProvider.GetServices<IGenericInterFace2>().ToList();
        var scope = serviceProvider.CreateScope();
        var scopedInstances = scope.ServiceProvider.GetServices<IGenericInterFace2>().ToList();

        // Assert
        instances.Should().NotBeNullOrEmpty();
        instances.Should().HaveCount(2);
        instances.Should().Contain(x => x.GetType() == typeof(ServiceWithGenericInterface2));
        instances.Should().Contain(x => x.GetType() == typeof(Service2WithGenericInterface2));

        scopedInstances.Should().NotBeNullOrEmpty();
        scopedInstances.Should().HaveCount(2);
        scopedInstances.Should().Contain(x => x.GetType() == typeof(ServiceWithGenericInterface2));
        scopedInstances.Should().Contain(x => x.GetType() == typeof(Service2WithGenericInterface2));
    }

    [Fact]
    public void ShouldRegisterMultipleServices_WithGenericSingletonInterface_AsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(SingletonWithGenericInterface1).Assembly;

        // Act
        services.AddBindicate(assembly);
        var serviceProvider = services.BuildServiceProvider();
        var instances = serviceProvider.GetServices<IGenericSingletonInterface>().ToList();
        var secondInstances = serviceProvider.GetServices<IGenericSingletonInterface>().ToList();

        // Assert
        instances.Should().NotBeNullOrEmpty();
        instances.Should().HaveCount(2);
        instances.Should().Contain(x => x.GetType() == typeof(SingletonWithGenericInterface1));
        instances.Should().Contain(x => x.GetType() == typeof(Singleton2WithGenericInterface1));

        secondInstances.Should().NotBeNullOrEmpty();
        secondInstances.Should().HaveCount(2);

        // Test if instances are singleton
        instances[0].Should().BeSameAs(secondInstances[0]);
        instances[1].Should().BeSameAs(secondInstances[1]);
    }

    [Fact]
    public void ShouldShareSingletonInstances_AcrossDifferentScopes()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(SingletonWithGenericInterface1).Assembly;

        // Act
        services.AddBindicate(assembly);
        var serviceProvider = services.BuildServiceProvider();
        var rootInstances = serviceProvider.GetServices<IGenericSingletonInterface>().ToList();

        using (var scope1 = serviceProvider.CreateScope())
        {
            var scopedInstances1 = scope1.ServiceProvider.GetServices<IGenericSingletonInterface>().ToList();

            using (var scope2 = serviceProvider.CreateScope())
            {
                var scopedInstances2 = scope2.ServiceProvider.GetServices<IGenericSingletonInterface>().ToList();

                // Assert that instances in the two different scopes are the same, confirming singleton lifetime
                scopedInstances1[0].Should().BeSameAs(scopedInstances2[0]);
                scopedInstances1[1].Should().BeSameAs(scopedInstances2[1]);

                // Assert that scoped instances are the same as root instances
                scopedInstances1[0].Should().BeSameAs(rootInstances[0]);
                scopedInstances1[1].Should().BeSameAs(rootInstances[1]);
            }
        }
    }
}

[AddGenericInterface]
public interface IGenericInterFace1 { }

[AddGenericInterface]
public interface IGenericInterFace2 { }

[AddGenericInterface]
public interface IGenericSingletonInterface { }

[AddScoped(typeof(IGenericInterFace1))]
public class ServiceWithGenericInterface1 : IGenericInterFace1 { }

[AddScoped(typeof(IGenericInterFace1))]
public class Service2WithGenericInterface1 : IGenericInterFace1 { }

[AddSingleton(typeof(IGenericSingletonInterface))]
public class SingletonWithGenericInterface1 : IGenericSingletonInterface { }

[AddSingleton(typeof(IGenericSingletonInterface))]
public class Singleton2WithGenericInterface1 : IGenericSingletonInterface { }

[AddScoped(typeof(IGenericInterFace2))]
public class ServiceWithGenericInterface2 : IGenericInterFace2 { }

[AddScoped(typeof(IGenericInterFace2))]
public class Service2WithGenericInterface2 : IGenericInterFace2 { }