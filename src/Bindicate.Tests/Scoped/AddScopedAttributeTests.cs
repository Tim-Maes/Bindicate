using Bindicate.Attributes;
using Bindicate.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace Bindicate.Tests.ScopedTests;

public class AddScopedAttributeTests
{
    private readonly Assembly _testAssembly = typeof(AddScopedAttributeTests).Assembly;

    [Fact]
    public void AddScoped_WithInterface_RegistersCorrectly()
    {
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<IScopedInterface>();

            service.Should().NotBeNull().And.BeOfType<ScopedWithInterface>();
        }
    }

    [Fact]
    public void AddScoped_RegistersCorrectly()
    {
        var services = new ServiceCollection();
        services.AddAutowiringForAssembly(_testAssembly);
        var serviceProvider = services.BuildServiceProvider();

        using (var scope = serviceProvider.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<SimpleScopedClass>();

            service.Should().NotBeNull().And.BeOfType<SimpleScopedClass>();
        }
    }
}

[AddScoped]
public class SimpleScopedClass { }

public interface IScopedInterface { }

[AddScoped(typeof(IScopedInterface))]
public class ScopedWithInterface : IScopedInterface { }
