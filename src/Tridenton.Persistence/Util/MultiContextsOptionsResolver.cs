using System.Collections.Concurrent;

namespace Tridenton.Persistence;

internal sealed class MultiContextsOptionsResolver
{
    private static readonly Lazy<MultiContextsOptionsResolver> _instance = new(() => new MultiContextsOptionsResolver());

    private readonly ConcurrentDictionary<Type, DataContextOptionsBuilder> _contextsOptions;
    private readonly object _locker = new();

    internal static MultiContextsOptionsResolver Instance => _instance.Value;

    private MultiContextsOptionsResolver()
    {
        _contextsOptions = new();
    }

    internal void SetOptionsPerContext<TDataContext>(DataContextOptionsBuilder options)
        where TDataContext : DataContext
    {
        SetOptions(typeof(TDataContext), options);
    }

    internal DataContextOptionsBuilder GetOptions<TDataContext>()
        where TDataContext : DataContext
    {
        return GetOptions(typeof(TDataContext));
    }

    internal DataContextOptionsBuilder GetOptions<TDataContext>(TDataContext context)
        where TDataContext : DataContext
    {
        return GetOptions(context.GetType());
    }

    internal DataContextOptionsBuilder GetOptions(Type contextType)
    {
        if (!_contextsOptions.TryGetValue(contextType, out var options))
        {
            options = new();
            SetOptions(contextType, options);
        }

        return options;
    }

    private void SetOptions(Type dataContextType, DataContextOptionsBuilder options)
    {
        lock (_locker)
        {
            _contextsOptions[dataContextType] = options;
        }
    }
}