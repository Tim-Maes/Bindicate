# Bindicate 🧷

Easily register your services in .NET's built-in IoC container with attribute directives.

## Features 🌟

- Automatic registration of services using custom attributes.
- Supports Transient, Scoped, and Singleton service lifetimes.
- No need for explicit interface specification for class-only registrations.
- Provides clear visibility and reduces boilerplate code.
- Simple integration with the built-in .NET IoC container..

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

### Add Bindicate
Update startup or service configuration and call the `AddBindicate()` method
```csharp
services.AddBindicate(Assembly.GetExecutingAssembly());
```
### Decorate your services:

**For class-only registrations:**

```csharp
[AddTransient]
public class SimpleTaskRunner
{
    public void RunTask()
    {
        // ...
    }
}
```

**When using interfaces:**

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
