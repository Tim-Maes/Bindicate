using Bindicate.Attributes;

namespace Bindicate.Tests.Generic;

public class BaseEntity
{
    public int Id { get; set; }
}

public class Customer : BaseEntity
{
    public string Name { get; set; }
    public string Country { get; set; }
    public string Email { get; set; }
}

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Price { get; set; }
}

[RegisterGenericInterface]
public interface IRepository<T> where T : BaseEntity
{
    void add(T entity);
}

[AddScoped(typeof(IRepository<>))]
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    public Repository()
    {
    }

    public void add(T entity)
    {
        Console.WriteLine(entity);
    }
}