namespace Tridenton.CQRS;

/// <summary>
///     Represents an async continuation for the next task to execute in the middleware
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
/// <returns>
///     Awaitable task returning a <typeparamref name="TResponse" />
/// </returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
///     Represents an async continuation for the next task to execute in the middleware
/// </summary>
/// <returns>
///     Awaitable task
/// </returns>
public delegate Task RequestHandlerDelegate();