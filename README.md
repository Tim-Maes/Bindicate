![image](https://github.com/Tim-Maes/Bindicate/assets/91606949/b2779c49-3d84-43ef-ad98-93f1108e3aa6)

```
'A blend of "Bind" and "Indicate"'.
```
![NuGet](https://img.shields.io/nuget/v/Bindicate.svg) ![NuGet](https://img.shields.io/nuget/dt/Bindicate.svg)

## Features 🌟

- Automatic registration of (keyed) services using custom attributes.
- Automatic registration and configuration of options via `IOptions<T>`.
- Provides clear visibility and reduces boilerplate code.
- Simple integration with the built-in .NET IoC container.
- Supports Decorators

### Supported types
<center>

| **Type**           | **Available** |  **Keyed (.NET 8)** | Decorators |Interceptors | Activators |
|--------------------|----------|------------------------------|---------|------------|------------|
|AddTransient        |✔️        |✔️                            | ✔️|❌ |❌ |
|TryAddTransient     |✔️        |❌                            | ❌| ❌|❌ |
|AddScoped           |✔️        |✔️                             |✔️ | ❌| ❌|
|TryAddScoped        |✔️        |❌                            |❌ |❌ |❌ |
|AddSingleton        |✔️        |✔️                            | ✔️| ❌| ❌|
|TryAddSingleton     |✔️        |❌                            |❌ |❌ |❌ |
|TryAddEnumerable    |✔️         |❌                           | ❌| ❌|❌ |
</center>

## Installation 📦

### Via NuGet

```bash
Install-Package Bindicate
```
or
```
dotnet add package Bindicate
```
## Usage

You can check out the [example project](https://github.com/Tim-Maes/Bindicate.Example) too!

### Autowire dependencies

**Register Services**

Add this line in a project to register all decorated services. You can repeat this line and pass any assembly.
To also configure options, use `.WithOptions()`.
You can also use the `ServiceCollectionExtension` pattern and use `IConfiguration` as a parameters for your extension method if they have options to register.

**Example in host project**
```csharp
// Register all decorated services in the current project
builder.Services
    .AddAutowiringForAssembly(Assembly.GetExecutingAssembly())
    .Register();

// Also register Keyed Services (.NET 8)
builder.Services
    .AddAutowiringForAssembly(Assembly.GetExecutingAssembly())
    .ForKeyedServices()
    .Register();

// Also register Options as IOptions<T>
builder.Services
    .AddAutowiringForAssembly(Assembly.GetExecutingAssembly())
    .ForKeyedServices()
    .WithOptions(Configuration)  //Pass builder.Configuration here
    .Register();
```

## Decorate your services:

### Basic usage

**For class-only registrations:**

Simple decorate your class with the attribute to register. You can use an attribute for a specific lifetime.

```csharp
[AddTransient]
public class SimpleTaskRunner
{
    public void RunTask()
    {
        // ...
    }
}

[TryAddSingleton]
public class SimpleService
{
    public void DoThing()
    {
        // ...
    }
}
```

**When using interfaces:**

Decorate your class with the attribute and provide the interface

```csharp
[AddScoped(typeof(IMyTaskRunner))]
public class TaskRunner : IMyTaskRunner
{
    public void Run()
    {
        // ...
    }
}

public interface IMyTaskRunner
{
    void Run();
}
```

**When using keyed services:**

Decorate your class with the attribute and provide the key

```csharp
[AddKeyedScoped("myKey")]
public class KeyedService
{
	public void Run()
	{
		// ...
	}
}

[AddKeyedScoped("key", typeof(IKeyedService))]
public class KeyedService : IKeyedService
{
	public void Run()
	{
		// ...
	}
}

[AddKeyedScoped("anotherKey", typeof(IKeyedService))]
public class AnotherKeyedService : IKeyedService
{
	public void Run()
	{
		// ...
	}
}
```

### Options Registration

Decorate your class containing the options with `[RegisterOptions]` and specify the corresponding section in `appsettings.json`.

```csharp
[RegisterOptions("testOptions")]
public class TestOptions
{
    public string Test { get; set; } = "";
}

//appsettings.json:
{
  "testOptions": {
    "test": "test"
  }
}
```

Now you can use this value when injecting `IOptions<TestOptions>` in your service

### Generics

**Define a generic interface:**

Decorate the generic interface with the `[RegisterGenericInterface]` attribute.

```csharp
[RegisterGenericInterface]
public interface IRepository<T> where T : BaseEntity
{
    void add(T entity);
}
```

**Create the implementation:**
```csharp
[AddTransient(typeof(IRepository<>))]
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    public Repository()
    {
    }

    public void add(T entity)
    {
        // Implementation here
    }
}
```

**How to use** 

You can now resolve instances of this type from `IServiceProvider`
```csharp
var customerRepo = serviceProvider.GetService<IRepository<Customer>>();
var productRepo = serviceProvider.GetService<IRepository<Product>>();
```

Both customerRepo and productRepo will be instances of Repository<T> but will operate on Customer and Product types, respectively.

### Decorators

Bindicate allows you to register decorators using attributes. Decorators wrap existing services to add additional behavior while preserving the original service's interface.

#### Defining a decorator:

Decorate your class with the `[RegisterDecorator]` attribute, specifying the service type it decorates.

```csharp
[RegisterDecorator(typeof(IMyService))]
public class MyServiceDecorator : IMyService
{
    private readonly IMyService _innerService;

    public MyServiceDecorator(IMyService innerService)
    {
        _innerService = innerService;
    }

    public void DoSomething()
    {
        // Add pre-processing logic here

        _innerService.DoSomething();

        // Add post-processing logic here
    }
}
```

#### Registering decorators:

Decorators are automatically applied when you call `.Register()` in your service registration.

#### Order of decorators:

If you have multiple decorators for the same service, you can specify the order in which they are applied using the `Order` parameter.

```csharp
[RegisterDecorator(typeof(IMyService), Order = 1)]
public class FirstDecorator : IMyService
{
    // ...
}

[RegisterDecorator(typeof(IMyService), Order = 2)]
public class SecondDecorator : IMyService
{
    // ...
}
```
Decorators with lower `Order` values are applied first. In this example, `FirstDecorator` will wrap `MyService`, and `SecondDecorator` will wrap `FirstDecorator`.

**Using the decorated service:**

When you resolve `IMyService` from the service provider, you will get the outermost decorator.

```csharp
var myService = serviceProvider.GetRequiredService<IMyService>();
myService.DoSomething();
```

**Example with a logging decorator:**

```csharp
[RegisterDecorator(typeof(IMyService))]
public class LoggingDecorator : IMyService
{
    private readonly IMyService _innerService;

    public LoggingDecorator(IMyService innerService)
    {
        _innerService = innerService;
    }

    public void DoSomething()
    {
        Console.WriteLine("Before DoSomething");
        _innerService.DoSomething();
        Console.WriteLine("After DoSomething");
    }
}
```

### Decorators with generics:

You can also create decorators for generic services.

```csharp
[RegisterDecorator(typeof(IRepository<>))]
public class RepositoryLoggingDecorator<T> : IRepository<T> where T : BaseEntity
{
    private readonly IRepository<T> _innerRepository;

    public RepositoryLoggingDecorator(IRepository<T> innerRepository)
    {
        _innerRepository = innerRepository;
    }

    public void Add(T entity)
    {
        Console.WriteLine($"Adding entity of type {typeof(T).Name}");
        _innerRepository.Add(entity);
        Console.WriteLine($"Added entity of type {typeof(T).Name}");
    }
}
```

Now, when you resolve `IRepository<Customer>` or `IRepository<Product>`, the `RepositoryLoggingDecorator<T>` will be applied.

## License

This project is licensed under the MIT license.
