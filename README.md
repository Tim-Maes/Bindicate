# Bindicate 🧷

```
'A blend of "Bind" and "Indicate"'.
```

Easily register your services in .NET's built-in IoC container with attribute directives.

## Features 🌟

- Automatic registration of services using custom attributes.
- No need for explicit interface specification for class-only registrations.
- Provides clear visibility and reduces boilerplate code.
- Simple integration with the built-in .NET IoC container.

### Supported types
<center>

| Type           | Supported|
|----------------|----------|
|AddTransient    |✔️        |
|TryAddTransient |✔️        |
|AddScoped       |✔️        |
|TryAddScoped    |✔️        |
|AddSingleton    |✔️        |
|TryAddSingleton |✔️        |
|TryAddEnumerable|❌       |
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

### Add Bindicate

Register Bindicate inside your startup class, or inside your project's `ServiceCollectionExtension`

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
