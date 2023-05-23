using Microsoft.EntityFrameworkCore;

namespace Tridenton.Persistence;

public sealed class DataContextOptionsBuilder : DbContextOptionsBuilder
{
    internal bool UseRequestsLogsTable { get; private set; }

    internal Type? RequestsLogsTableType { get; private set; }

    internal bool CanCreateTableAtRuntime { get; private set; }

    internal bool LifecycleQueryFilterApplied { get; private set; }

    internal bool IsDatabaseCreatedVerificationEnabled { get; private set; }

    internal Assembly[] AdditionalEnumerationsAssemblies { get; private set; }

    internal DataContextOptionsBuilder()
    {
        AdditionalEnumerationsAssemblies = Array.Empty<Assembly>();
    }

    internal void CreateRequestsLogsTable(Type requestsLogsTableType)
    {
        RequestsLogsTableType = requestsLogsTableType;
        UseRequestsLogsTable = true;
    }

    public DataContextOptionsBuilder WithDatabaseCreationVerification()
    {
        IsDatabaseCreatedVerificationEnabled = true;
        return this;
    }

    public DataContextOptionsBuilder WithTablesCreationAtRuntime()
    {
        CanCreateTableAtRuntime = true;
        return this;
    }

    public DataContextOptionsBuilder WithLifecycleQueryFilter()
    {
        LifecycleQueryFilterApplied = true;
        return this;
    }

    public DataContextOptionsBuilder WithEnumerationsFromAssemblies(params Assembly[] assemblies)
    {
        AdditionalEnumerationsAssemblies = assemblies;
        return this;
    }
}