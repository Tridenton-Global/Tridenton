namespace Tridenton.CQRS;

public abstract class BaseHandler : AbstractService
{
    protected readonly IOrchestrator Orchestrator;

    protected BaseHandler(IServiceProvider services) : base(services)
    {
        Orchestrator = GetService<IOrchestrator>();
    }
}