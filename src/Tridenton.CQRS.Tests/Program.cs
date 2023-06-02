using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tridenton.CQRS;
using Tridenton.CQRS.Tests;
using Tridenton.Extensions.CQRS;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddCQRS(options =>
        {
            options.AddAssemblies(Assembly.GetExecutingAssembly());
            options.AddMiddleware(typeof(MiddlewareTest1<,>));
            options.AddMiddleware(typeof(MiddlewareTest2<,>));
            options.AddMiddleware(typeof(MiddlewareTest3<,>));
            options.AddMiddleware(typeof(MiddlewareTest4<,>));
        });

        services.AddTransient<ITransientService, TransientService>();
    })
    .Build();

var orchestrator = host.Services.GetService<IOrchestrator>()!;

//await orchestrator.PublishAsync(new NotificationTest());
//await orchestrator.SendAsync(new CQRSTestRequest());
var response = await orchestrator.SendAsync<CQRSTestGenericRequest, CQRSTestGenericResponse>(new());

Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine(response.Value);

Console.ReadKey();

//await host.RunAsync();