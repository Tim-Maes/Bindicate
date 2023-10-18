# Bindicate 🧷

```
'A blend of "Bind" and "Indicate"'.
```
![NuGet](https://img.shields.io/nuget/v/Bindicate.svg)

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

### Autowire dependencies

**Register Services per Assembly**

Add this line in a project to register all decorated services. You can repeat this line and pass any assembly.

```csharp
// Register all types in current project
services.AddAutowiringForAssembly(Assembly.GetExecutingAssembly());

// Register types from referenced project
services.AddAutowiringForAssembly(Assembly.GetAssembly(typeof(IInterface))); 
```

**Register Services Across Multiple Assemblies**

If you want to scan and register services across all loaded assemblies, you can do so by adding the following line in your hosting project:

***Note** that this might not work if not all assemblies are loaded at this point in startup configuration*!

```csharp
// Trigger loading of unloaded assemblies to be able to use AddAutowiring:
var triggerAssembly1 = typeof(ProjectName.SomeType);
var triggerAssembly2 = typeof(OtherProjectName.SomeOtherType);

services.AddAutowiring();

//Or just use AddAutowiringForAssembly method
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
