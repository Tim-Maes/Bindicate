using Bindicate.Attributes;

namespace Bindicate.Tests.Generic;

[RegisterGenericInterface]
public interface ITransientRepository<T> where T : BaseEntity
{
    void add(T entity);
}

[AddTransient(typeof(ITransientRepository<>))]
public class TransientRepository<T> : ITransientRepository<T> where T : BaseEntity
{
    public TransientRepository()
    {
    }

    public void add(T entity)
    {
        Console.WriteLine(entity);
    }
}