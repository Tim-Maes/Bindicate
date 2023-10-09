# Bindicate 🧷

Easily register your services in .NET's built-in IoC container with attribute directives.

## Features 🌟

- Automatic registration of services using custom attributes.
- Supports Transient, Scoped, and Singleton service lifetimes.
- Provides clear visibility and reduces boilerplate code.
- Simple integration with the built-in .NET IoC container.

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

### Update startup or service configuration and call the `AddBindicate()` method
```
services.AddAutoServiceCollection(Assembly.GetExecutingAssembly());
```
### Decorate your services:

Use the [RegisterService] attribute on your service classes, specifying the interface and the desired lifetime.

```
[RegisterService(typeof(IMyService), LifeTime.Transient)]
public class MyService : IMyService
{
    public void DoWork()
    {
        // ...
    }
}

public interface IMyService
{
    void DoWork();
}

```

## License

This project is licensed under the MIT license.
