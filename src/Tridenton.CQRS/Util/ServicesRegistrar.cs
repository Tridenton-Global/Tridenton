using System.Collections.Concurrent;

namespace Tridenton.CQRS;

internal sealed class ServicesRegistrar
{
    private static readonly Lazy<ServicesRegistrar> _instance = new(() => new ServicesRegistrar());

    internal static ServicesRegistrar Instance => _instance.Value;

    private readonly ConcurrentDictionary<Type, Type> _requestsHandlers;
    private readonly List<KeyValuePair<Type, Type>> _notificationsHandlers;

    private ServicesRegistrar()
    {
        _requestsHandlers = new();
        _notificationsHandlers = new();
    }

    internal void AddRequestHandler(Type requestType, Type handlerType)
    {
        _requestsHandlers.TryAdd(requestType, handlerType);
    }

    internal void AddNotificationHandlers(Type notificationType, Type[] handlerTypes)
    {
        if (!handlerTypes.Any()) return;

        for (long i = 0; i < handlerTypes.LongLength; i++)
        {
            _notificationsHandlers.Add(new KeyValuePair<Type, Type>(notificationType, handlerTypes[i]));
        }
    }

    internal Type GetRequestHandler(Type requestType) => _requestsHandlers[requestType];

    internal Type[] GetNotificationHandlers(Type notificationType) => _notificationsHandlers.Where(h => h.Key == notificationType).Select(h => h.Value).ToArray();
}