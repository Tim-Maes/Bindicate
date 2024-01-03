# Bindicate 🧷

```
'A blend of "Bind" and "Indicate"'.
```
![NuGet](https://img.shields.io/nuget/v/Bindicate.svg) ![NuGet](https://img.shields.io/nuget/dt/Bindicate.svg)

## Features 🌟

- Automatic registration of services using custom attributes.
- Automatic registration and configuration of options via `IOptions<T>`.
- Provides clear visibility and reduces boilerplate code.
- Simple integration with the built-in .NET IoC container.

### Supported types
<center>

| **Type**           | **Available** |  **Keyed (.NET 8)** |
|--------------------|----------|------------------------------|
|AddTransient        |✔️        |✔️                            |
|TryAddTransient     |✔️        |❌                            |
|AddScoped           |✔️        |✔️                             |
|TryAddScoped        |✔️        |❌                            |
|AddSingleton        |✔️        |✔️                            |
|TryAddSingleton     |✔️        |❌                            |
|TryAddEnumerable    |❌        |❌                           |
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

// Register types and options from referenced project
builder.Services
    .AddAutowiringForAssembly(Assembly.GetAssembly(typeof(IInterface)))
    .WithOptions(Configuration)
    .Register();
```

**Example with ServiceCollectionExtensions**

```csharp
// Hosting project:
var configuration = builder.Configuration;

builder.Services.AddSecondProject(configuration);

// In other project
public static IServiceCollection AddSecondProject(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutowiringForAssembly(Assembly.GetExecutingAssembly())
                .WithOptions(configuration)
                .Register();

        return services;
    }
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
[AddScoped("myKey")]
public class KeyedService
{
	public void Run()
	{
		// ...
	}
}

[AddScoped("key", typeof(IKeyedService))]
public class KeyedService : IKeyedService
{
	public void Run()
	{
		// ...
	}
}

[AddScoped("anotherKey", typeof(IKeyedService))]
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

## License

This project is licensed under the MIT license.
