using Bindicate.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bindicate.Tests.Generic;

public class GenericRepositoryRegistrationTests
{
    [Fact]
    public void ShouldRegisterGenericRepository_ForCustomerAndProduct_AsTransient()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(TransientRepository<>).Assembly;

        // Act
        services.AddAutowiringForAssembly(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var instanceForCustomer = serviceProvider.GetService<ITransientRepository<Customer>>();
        var instanceForProduct = serviceProvider.GetService<ITransientRepository<Product>>();

        // All instances should exist
        instanceForCustomer.Should().NotBeNull();
        instanceForCustomer.Should().BeAssignableTo<ITransientRepository<Customer>>();

        instanceForProduct.Should().NotBeNull();
        instanceForProduct.Should().BeAssignableTo<ITransientRepository<Product>>();
    }

    [Fact]
    public void ShouldRegisterGenericRepository_ForCustomer_AsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(Repository<>).Assembly;

        // Act
        services.AddAutowiringForAssembly(assembly);
        var serviceProvider = services.BuildServiceProvider();

        IRepository<Customer> instance1, instance2, instance3;

        // Resolve two instances from the same scope
        using (var scope1 = serviceProvider.CreateScope())
        {
            var scopedProvider1 = scope1.ServiceProvider;
            instance1 = scopedProvider1.GetService<IRepository<Customer>>();
            instance2 = scopedProvider1.GetService<IRepository<Customer>>();

            // Assert that both instances are the same within the same scope
            instance1.Should().Be(instance2);
        }

        // Resolve one instance from a different scope
        using (var scope2 = serviceProvider.CreateScope())
        {
            var scopedProvider2 = scope2.ServiceProvider;
            instance3 = scopedProvider2.GetService<IRepository<Customer>>();

            // Assert that this instance is different from the instances from the first scope
            instance3.Should().NotBe(instance1);
        }
    }

    [Fact]
    public void ShouldRegisterGenericRepository_ForCustomer_AsTransient()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = typeof(TransientRepository<>).Assembly;

        // Act
        services.AddAutowiringForAssembly(assembly);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var instance1 = serviceProvider.GetService<ITransientRepository<Customer>>();
        var instance2 = serviceProvider.GetService<ITransientRepository<Customer>>();
        var instance3 = serviceProvider.GetService<ITransientRepository<Customer>>();

        // All instances should exist
        instance1.Should().NotBeNull();
        instance2.Should().NotBeNull();
        instance3.Should().NotBeNull();

        // All instances should be unique
        instance1.Should().NotBeSameAs(instance2);
        instance2.Should().NotBeSameAs(instance3);
        instance1.Should().NotBeSameAs(instance3);
    }
}