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

You can check out the [documentation](https://github.com/Tim-Maes/Bindicate/wiki) for examples and more information on how to use Bindicate.

### Quick overview

Add this line in a project to register all decorated services.
To also [configure options](https://github.com/Tim-Maes/Bindicate/wiki/03.-Registering-Options), use `.WithOptions()`.
You can also use the `ServiceCollectionExtension` pattern and use `IConfiguration` as a parameters for your extension method if they have options to register.

**Example in host project**
```csharp
// Register all decorated services for the current assembly
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

**Example usage**
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

## License

This project is licensed under the MIT license.
