using Tridenton.CQRSPersistenceInteraction;

namespace Tridenton.CQRS;

public sealed class CQRSOptionsBuilder
{
    internal List<Assembly> Assemblies { get; private set; }

    internal List<Type> Middlewares { get; private set; }

    internal bool SavesLogs { get; private set; }

    internal CQRSLogsOptionsBuilder LogsOptions { get; private set; }

    internal CQRSOptionsBuilder()
    {
        Assemblies = new();
        Middlewares = new();
        LogsOptions = new();
    }

    public CQRSOptionsBuilder AddMiddleware(Type middlewareType)
    {
        Middlewares.Add(middlewareType);
        return this;
    }

    public CQRSOptionsBuilder AddAssemblies(params Assembly[] assemblies)
    {
        Assemblies.AddRange(assemblies);
        return this;
    }

    public CQRSOptionsBuilder SaveRequestsLogs(Action<CQRSLogsOptionsBuilder> options)
    {
        options.Invoke(LogsOptions);

        SavesLogs = true;
        return this;
    }
}

public sealed class CQRSLogsOptionsBuilder : BaseCQRSOptionsBuilder
{
    private const string DefaultServerErrorMessage = "The request processing has failed because of an unknown error, exception or failure.";

    internal bool ServerErrorReplaced { get; private set; }

    internal string ServerErrorMessage { get; private set; }

    internal CQRSLogsOptionsBuilder()
    {
        ServerErrorMessage = DefaultServerErrorMessage;
    }

    public CQRSLogsOptionsBuilder WithHiddenResponseMetadata()
    {
        HideResponseMetadata();
        return this;
    }

    public CQRSLogsOptionsBuilder WithServerErrorMessage(string message = DefaultServerErrorMessage)
    {
        ServerErrorReplaced = true;
        ServerErrorMessage = message;
        return this;
    }
}