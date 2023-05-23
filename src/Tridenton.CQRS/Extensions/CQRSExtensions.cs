using Microsoft.Extensions.DependencyInjection;

namespace Tridenton.CQRS;

public static class CQRSExtensions
{
    public static IServiceCollection AddCQRS(this IServiceCollection services, Action<CQRSOptionsBuilder> options)
    {
        var settings = new CQRSOptionsBuilder();
        options.Invoke(settings);

        return services;
    }
}