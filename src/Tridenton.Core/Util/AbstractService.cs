using Microsoft.Extensions.DependencyInjection;

namespace Tridenton.Core.Util;

public abstract class AbstractService
{
    private readonly IServiceProvider _services;

    protected AbstractService(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    ///     Retrieves <typeparamref name="TService"/> from the <see cref="IServiceProvider"/> if it is present; otherwise, throws <see cref="InvalidOperationException"/>
    /// </summary>
    /// <typeparam name="TService">Service type</typeparam>
    /// <returns><typeparamref name="TService"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected TService GetService<TService>() where TService : class
    {
        var service = _services.GetService<TService>();

        return service is null ? throw new InvalidOperationException($"Unable to initialize instance of {typeof(TService).Name}") : service;
    }

    /// <summary>
    ///     Retrieves <typeparamref name="TService"/> from the <see cref="IServiceProvider"/> if it is present; otherwise, throws <see cref="InvalidOperationException"/>
    /// </summary>
    /// <typeparam name="TService">Service type</typeparam>
    /// <returns><typeparamref name="TService"/></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected TService GetService<TService>(Type serviceType) where TService : class
    {
        var service = _services.GetService(serviceType) ?? throw new InvalidOperationException($"Unable to initialize instance of {typeof(TService).Name}");

        return (service as TService)!;
    }

    /// <summary>
    ///     Get an enumeration of services of type <typeparamref name="TService"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns>An array of services of type <typeparamref name="TService"/>.</returns>
    protected TService[] GetServices<TService>() where TService : class
    {
        return _services.GetServices<TService>().ToArray();
    }

    /// <summary>
    ///     Get an enumeration of services of type <typeparamref name="TService"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns>An array of services of type <typeparamref name="TService"/>.</returns>
    protected TService[] GetServices<TService>(Type serviceType) where TService : class
    {
        return _services.GetServices(serviceType).Select(s => (s as TService)!).ToArray();
    }
}