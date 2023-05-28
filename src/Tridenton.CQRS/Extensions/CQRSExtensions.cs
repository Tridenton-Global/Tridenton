using Microsoft.Extensions.DependencyInjection;

namespace Tridenton.CQRS;

public static class CQRSExtensions
{
    public static IServiceCollection AddCQRS(this IServiceCollection services, Action<CQRSOptionsBuilder> options)
    {
        var settings = new CQRSOptionsBuilder();
        options.Invoke(settings);

        services.AddSingleton<IOrchestrator, Orchestrator>();

        var assemblies = settings.Assemblies.Distinct().ToArray();

        var definedTypes = assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => !t.IsAbstract);

        var requestsTypes = definedTypes
            .Where(t => t == typeof(TridentonRequest) || t == typeof(TridentonRequest<>))
            .ToArray();

        for (long i = 0; i < requestsTypes.LongLength; i++)
        {
            var requestType = requestsTypes[i];

            var requestHandlersTypes = definedTypes
                .Where(t => t == typeof(RequestHandler<>) && t.GetGenericArguments()[0] == requestType);

            if (requestType.IsGenericType)
            {
                var responseType = requestType.GetGenericArguments()[1];

                requestHandlersTypes = requestHandlersTypes
                    .Where(t => t.GetGenericArguments()[1] == responseType);
            }

            var requestHandlersTypesArray = requestHandlersTypes.ToArray();

            if (requestHandlersTypesArray.LongLength > 1)
            {
                throw new MoreThanOneRequestHandlerException(requestHandlersTypesArray[0]);
            }

            ServicesRegistrar.Instance.AddRequestHandler(requestType, requestHandlersTypesArray[0]);

            var notificationHandlers = definedTypes
                .Where(t => t == typeof(NotificationHandler<>) && t.GetGenericArguments()[0] == requestType)
                .ToArray();

            ServicesRegistrar.Instance.AddNotificationHandlers(requestType, notificationHandlers);
        }

        if (settings.SavesLogs)
        {

        }

        return services;
    }
}