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

public sealed class CQRSLogsOptionsBuilder : ResponseOptionsBuilder
{
    private const string DefaultServerErrorMessage = "The request processing has failed because of an unknown error, exception or failure.";

    internal bool ServerErrorReplaced { get; private set; }

    internal string ServerErrorMessage { get; private set; }

    internal CQRSMetricsDataContextOptionsBuilder DataContextOptions { get; private set; }

    internal CQRSLogsOptionsBuilder()
    {
        ServerErrorMessage = DefaultServerErrorMessage;
        DataContextOptions = new();
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

    //public CQRSLogsOptionsBuilder WithDataContext<TDataContext>()
    //    where TDataContext : DataContext
    //{
    //    DataContextOptions.DataContextType = typeof(TDataContext);

    //    return this;
    //}

    //public CQRSLogsOptionsBuilder UsingEntity<TRequestLog>()
    //    where TRequestLog : RequestLog
    //{
    //    DataContextOptions.RequestLogEntityType = typeof(TRequestLog);

    //    return this;
    //}
}

internal sealed class CQRSMetricsDataContextOptionsBuilder
{
    private Type? _dataContextType;
    internal Type? DataContextType
    {
        get => _dataContextType;
        set
        {
            if (_dataContextType is null)
            {
                _dataContextType = value;
            }
        }
    }

    //private Type _requestLogEntityType = typeof(RequestLog);
    //internal Type RequestLogEntityType
    //{
    //    get => _requestLogEntityType;
    //    set
    //    {
    //        _requestLogEntityType = value;
    //    }
    //}
}