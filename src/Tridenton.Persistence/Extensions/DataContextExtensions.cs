using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Tridenton.CQRSPersistenceInteraction;

namespace Tridenton.Extensions.Persistence;

public static class DataContextExtensions
{
    /// <summary>
    ///     Registers the given context as a service in the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TDataContext"></typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="options">Configuration options</param>
    /// <returns>
    ///     The same service collection so that multiple calls can be chained.
    /// </returns>
    public static IServiceCollection AddDataContext<TDataContext>(this IServiceCollection services, Action<DataContextOptionsBuilder>? options = null)
        where TDataContext : DataContext
    {
        var settings = new DataContextOptionsBuilder();
        options?.Invoke(settings);

        if (settings.CanCreateTableAtRuntime)
        {
            settings.ReplaceService<IModelCacheKeyFactory, DataContextModelCacheKeyFactory<TDataContext>>();
        }

        services.AddDbContext<TDataContext>(options => options = settings);

        MultiContextsOptionsResolver.Instance.SetOptionsPerContext<TDataContext>(settings);

        return services;
    }

    public static BaseCQRSOptionsBuilder WithDataContext<TDataContext>(this BaseCQRSOptionsBuilder options) where TDataContext : DataContext
    {
        options.DataContextType = typeof(TDataContext);

        return options;
    }

    public static BaseCQRSOptionsBuilder UsingEntity<TRequestLog>(this BaseCQRSOptionsBuilder options) where TRequestLog : RequestLog
    {
        options.RequestLogEntityType = typeof(TRequestLog);

        return options;
    }
}