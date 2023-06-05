using Microsoft.Extensions.DependencyInjection;
using Tridenton.CQRS;

namespace Tridenton.Extensions.CQRS;

public static class CQRSExtensions
{
    public static IServiceCollection AddCQRS(this IServiceCollection services, Action<CQRSOptionsBuilder> options)
    {
        var settings = new CQRSOptionsBuilder();
        options.Invoke(settings);

        ServicesRegistrar.Instance.PublicationBehavior = settings.PublicationBehavior;

        services.AddSingleton<IOrchestrator, Orchestrator>();

        var assemblies = settings.Assemblies.Distinct().ToArray();

        var definedTypes = assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => !t.IsAbstract);

        var requestsTypes = definedTypes
            .Where(t => t.BaseType == typeof(TridentonRequest) || HasGenericParent(t, typeof(TridentonRequest<>)))
            .ToArray();

        var requestsHandlersTypes = definedTypes
            .Where(t => HasGenericParent(t, typeof(RequestHandler<>)) || HasGenericParent(t, typeof(RequestHandler<,>)))
            .ToArray();

        for (long i = 0; i < requestsTypes.LongLength; i++)
        {
            var requestType = requestsTypes[i];

            var requestHandlerTypes = requestsHandlersTypes
                .Where(t => t.BaseType!.GenericTypeArguments[0] == requestType);

            if (requestType.BaseType!.IsGenericType)
            {
                var responseType = requestType.BaseType!.GenericTypeArguments[0];

                requestHandlerTypes = requestHandlerTypes
                    .Where(t => t.BaseType!.GenericTypeArguments[1] == responseType);
            }

            var requestHandlersTypesArray = requestHandlerTypes.ToArray();

            if (requestHandlersTypesArray.Any())
            {
                if (requestHandlersTypesArray.LongLength > 1)
                {
                    throw new MoreThanOneRequestHandlerException(requestType);
                }

                ServicesRegistrar.Instance.AddRequestHandler(requestType, requestHandlersTypesArray[0]);

                services.AddTransient(requestHandlersTypesArray[0]);
            }

            var notificationHandlers = definedTypes
                .Where(t => HasGenericParent(t, typeof(NotificationHandler<>)) && t.BaseType!.GenericTypeArguments[0] == requestType)
                .ToArray();

            ServicesRegistrar.Instance.AddNotificationHandlers(requestType, notificationHandlers);

            for (long j = 0; j < notificationHandlers.LongLength; j++)
            {
                services.AddTransient(notificationHandlers[j]);
            }
        }

        for (int i = 0; i < settings.Middlewares.Count; i++)
        {
            var middleware = settings.Middlewares.ElementAt(i);

            if (HasGenericParent((TypeInfo)middleware, typeof(CQRSMiddleware<>)))
            {
                services.AddTransient(typeof(CQRSMiddleware<>), middleware);
            }

            if (HasGenericParent((TypeInfo)middleware, typeof(CQRSMiddleware<,>)))
            {
                services.AddTransient(typeof(CQRSMiddleware<,>), middleware);
            }
        }

        if (settings.SavesLogs)
        {

        }

        return services;
    }

    private static bool HasGenericParent(TypeInfo type, Type genericType)
    {
        return type.BaseType is not null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == genericType;
    }
}