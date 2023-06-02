using Tridenton.Core.Models;

namespace Tridenton.CQRS.Tests;

public sealed class MiddlewareTest1<TRequest, TResponse> : CQRSMiddleware<TRequest, TResponse> where TRequest : TridentonRequest<TResponse> where TResponse : class
{
    public MiddlewareTest1(IServiceProvider services) : base(services)
    {
    }

    public override Task<TResponse> HandleAsync(IMiddlewareContext<TRequest, TResponse> context)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Executing 1 middleware");

        return context.Next.Invoke();
    }
}

public sealed class MiddlewareTest2<TRequest, TResponse> : CQRSMiddleware<TRequest, TResponse> where TRequest : TridentonRequest<TResponse> where TResponse : class
{
    public MiddlewareTest2(IServiceProvider services) : base(services)
    {
    }

    public override Task<TResponse> HandleAsync(IMiddlewareContext<TRequest, TResponse> context)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Executing 2 middleware");

        return context.Next.Invoke();
    }
}

public sealed class MiddlewareTest3<TRequest, TResponse> : CQRSMiddleware<TRequest, TResponse> where TRequest : TridentonRequest<TResponse> where TResponse : class
{
    public MiddlewareTest3(IServiceProvider services) : base(services)
    {
    }

    public override Task<TResponse> HandleAsync(IMiddlewareContext<TRequest, TResponse> context)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Executing 3 middleware");

        return context.Next.Invoke();
    }
}

public sealed class MiddlewareTest4<TRequest, TResponse> : CQRSMiddleware<TRequest, TResponse> where TRequest : TridentonRequest<TResponse> where TResponse : class
{
    public MiddlewareTest4(IServiceProvider services) : base(services)
    {
    }

    public override Task<TResponse> HandleAsync(IMiddlewareContext<TRequest, TResponse> context)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Executing 4 middleware");

        return context.Next.Invoke();
    }
}