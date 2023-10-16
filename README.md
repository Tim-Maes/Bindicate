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

## Basic usage

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

## Generics

**Define a generic interface:**

Decorate the generic interface with the [RegisterGenericInterface] attribute.

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
