using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tridenton.CQRS;
using Tridenton.CQRS.Tests;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddCQRS(options =>
        {
            options.AddAssemblies(Assembly.GetExecutingAssembly());
        });

        services.AddTransient<ITransientService, TransientService>();
    })
    .Build();

var orchestrator = host.Services.GetService<IOrchestrator>()!;

await orchestrator.PublishAsync(new NotificationTest());
//await orchestrator.SendAsync(new CQRSTestRequest());
//var response = await orchestrator.SendAsync<CQRSTestGenericRequest, CQRSTestGenericResponse>(new());

await host.RunAsync();