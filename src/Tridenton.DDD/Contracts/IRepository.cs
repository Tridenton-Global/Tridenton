namespace Tridenton.DDD.Contracts;

public interface IRepository<TEntity> : IDisposable, IAsyncDisposable where TEntity : Entity
{
}