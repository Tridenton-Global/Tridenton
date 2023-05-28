using Microsoft.Extensions.DependencyInjection;

namespace Tridenton.CQRS;

public static class CQRSExtensions
{
    public static IServiceCollection AddCQRS(this IServiceCollection services, Action<CQRSOptionsBuilder> options)
    {
        var settings = new CQRSOptionsBuilder();
        options.Invoke(settings);

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

            if (requestType.IsGenericType)
            {

            }

            var notificationHandlers = definedTypes
                .Where(t => t == typeof(NotificationHandler<>) && t.GetGenericArguments()[0] == requestType)
                .ToArray();

            ServicesRegistrar.Instance.AddNotificationHandlers(requestType, notificationHandlers);
        }

        return services;
    }
}