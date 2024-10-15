using Bindicate.Attributes.Enumerable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bindicate.Tests.Enumerable;

public class TryAddEnumberableTests
{
    private readonly Assembly _testAssembly = typeof(TryAddEnumberableTests).Assembly;

    [Fact]
    public void TryAddEnumerable_MultipleImplementations_RegistersAll()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly).Register();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var myServices = serviceProvider.GetServices<IMyService>().ToList();

        // Assert
        myServices.Should().NotBeNull();
        myServices.Count.Should().Be(2);
        myServices.Should().ContainSingle(s => s.GetType() == typeof(MyServiceA));
        myServices.Should().ContainSingle(s => s.GetType() == typeof(MyServiceB));
    }

    [Fact]
    public void TryAddEnumerable_DuplicateImplementation_NotRegisteredTwice()
    {
        // Arrange
        var services = new ServiceCollection();

        // Manually add MyServiceA to simulate duplicate registration
        services.TryAddEnumerable(ServiceDescriptor.Transient<IMyService, MyServiceA>());
        services.AddAutowiringForAssembly(_testAssembly).Register();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var myServices = serviceProvider.GetServices<IMyService>().ToList();

        // Assert
        myServices.Should().NotBeNull();
        myServices.Count().Should().Be(2);
        myServices.Should().ContainSingle(s => s.GetType() == typeof(MyServiceA));
        myServices.Should().ContainSingle(s => s.GetType() == typeof(MyServiceB));
    }

    [Fact]
    public void TryAddEnumerable_Lifetime_IsRespected()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly).Register();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var scope1Services = serviceProvider.GetServices<IMyService>().ToList();
        var scope2Services = serviceProvider.GetServices<IMyService>().ToList();

        // Assert
        // Since the services are registered as Transient, each resolve should return new instances
        scope1Services.Should().NotBeSameAs(scope2Services);

        var serviceA1 = scope1Services.First(s => s.GetType() == typeof(MyServiceA));
        var serviceA2 = scope2Services.First(s => s.GetType() == typeof(MyServiceA));

        serviceA1.Should().NotBeSameAs(serviceA2);
    }
}

public interface IMyService
{
    void Execute();
}

[TryAddEnumerable(Lifetime.TryAddEnumerableTransient, typeof(IMyService))]
public class MyServiceA : IMyService
{
    public void Execute()
    {
    }
}

[TryAddEnumerable(Lifetime.TryAddEnumerableTransient, typeof(IMyService))]
public class MyServiceB : IMyService
{
    public void Execute()
    {
    }
}
