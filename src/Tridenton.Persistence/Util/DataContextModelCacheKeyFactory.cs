using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Tridenton.Persistence;

internal sealed class DataContextModelCacheKeyFactory<TDataContext> : IModelCacheKeyFactory
    where TDataContext : DataContext
{
    public object Create(DbContext context, bool designTime)
    {
        return context is TDataContext dataContext ? (dataContext, designTime) : (object)context.GetType();
    }
}