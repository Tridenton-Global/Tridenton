using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Tridenton.Extensions.Persistence;

namespace Tridenton.Persistence;

public abstract class DataContext : DbContext
{
    protected readonly DataContextOptionsBuilder Options;

    public DataContext()
    {
        Options = MultiContextsOptionsResolver.Instance.GetOptions(this);
        VerifyDatabaseCreated();
    }

    protected DataContext(DbContextOptions options) : base(options)
    {
        Options = MultiContextsOptionsResolver.Instance.GetOptions(this);
        VerifyDatabaseCreated();
    }

    #region Additional queryables

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IQueryable GetQueryable(Type type)
    {
        MethodInfo method = GetType().GetMethods().FirstOrDefault(m => m.Name == nameof(DbContext.Set) && m.IsGenericMethod)!.MakeGenericMethod(type);

        var query = (IQueryable)method.Invoke(this, null)!;

        return query;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="item"></param>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public IQueryable<TEntity> GetQueryByProperty<TEntity>(TEntity item, string property, object? value) where TEntity : class
    {
        return GetQueryByProperty<TEntity>(item.GetType(), property, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="type"></param>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public IQueryable<TEntity> GetQueryByProperty<TEntity>(Type type, string property, object? value) where TEntity : class
    {
        return GetQueryByProperty(type, property, value).Cast<TEntity>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public IQueryable GetQueryByProperty(Type type, string property, object? value)
    {
        return GetQueryByProperties(type, new()
        {
            { property, value }
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public IQueryable GetQueryByProperties(Type type, Dictionary<string, object?> properties)
    {
        var query = GetQueryable(type);

        if (properties is null || !properties.Any()) return query;

        var predicateParts = properties.Select((p, i) => $"{p.Key} = @{i}");
        var predicate = string.Join(PaginationConstants.And, predicateParts);

        return query.Where(predicate, properties.Select(p => p.Value).ToArray());
    }

    #endregion

    #region Get by ID

    /// <summary>
    ///     Asynchronously retrieves <typeparamref name="TEntity"/> by ID
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="id">Entity ID</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains <typeparamref name="TEntity"/> if exists; otherwise, <see langword="null"/>
    /// </returns>
    public async ValueTask<TEntity?> GetByIDAsync<TEntity, TId>(TId id, CancellationToken cancellationToken = default)
        where TEntity : Entity
        where TId : struct
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await FindAsync<TEntity>(id);
    }

    #endregion

    #region Add

    /// <summary>
    ///     Begins tracking the given entity, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>
    ///     The <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    public override EntityEntry Add(object entity)
    {
        SetAsNewEntity(entity);

        return base.Add(entity);
    }

    /// <summary>
    ///     Begins tracking the given entity, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous Add operation. The task result contains the
    ///     <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry" /> for the entity. The entry provides access to change tracking
    ///     information and operations for the entity.
    /// </returns>
    /// <exception cref="T:System.OperationCanceledException">If the <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
    public override ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default)
    {
        SetAsNewEntity(entity);

        return base.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    ///     Begins tracking the given entity, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Added" /> state such that
    ///     they will be inserted into the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <returns>
    ///     The <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry`1" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
    {
        SetAsNewEntity(entity);

        return base.Add(entity);
    }

    /// <summary>
    ///     Begins tracking the given entity, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous Add operation. The task result contains the
    ///     <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry`1" /> for the entity. The entry provides access to change tracking
    ///     information and operations for the entity.
    /// </returns>
    /// <exception cref="T:System.OperationCanceledException">If the <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
    public override async ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
    {
        SetAsNewEntity(entity);

        return await base.AddAsync(entity, cancellationToken);
    }

    #endregion

    #region Add range

    /// <summary>
    ///     Begins tracking the given entities, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    public override void AddRange(params object[] entities)
    {
        for (long i = 0; i < entities.LongLength; i++)
        {
            var entity = entities[i];

            SetAsNewEntity(entity);
        }

        base.AddRange(entities);
    }

    /// <summary>
    ///     Begins tracking the given entities, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    public override void AddRange(IEnumerable<object> entities)
    {
        AddRange(entities.ToArray());
    }

    /// <summary>
    ///     Begins tracking the given entity, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public override Task AddRangeAsync(params object[] entities)
    {
        for (long i = 0; i < entities.LongLength; i++)
        {
            var entity = entities[i];

            SetAsNewEntity(entity);
        }

        return base.AddRangeAsync(entities);
    }

    /// <summary>
    ///     Begins tracking the given entity, and any other reachable entities that are
    ///     not already being tracked, in the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Added" /> state such that they will
    ///     be inserted into the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="T:System.OperationCanceledException">If the <see cref="T:System.Threading.CancellationToken" /> is canceled.</exception>
    public override Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
    {
        return AddRangeAsync(entities.ToArray());
    }

    #endregion

    #region Update

    /// <summary>
    ///     Begins tracking the given entity and entries reachable from the given entity using
    ///     the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Modified" /> state by default, but see below for cases
    ///     when a different state will be used.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>
    ///     The <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    public override EntityEntry Update(object entity)
    {
        SetAsModifiedEntity(entity);

        return base.Update(entity);
    }

    /// <summary>
    ///     Begins tracking the given entity and entries reachable from the given entity using
    ///     the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Modified" /> state by default, but see below for cases
    ///     when a different state will be used.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <returns>
    ///     The <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry`1" /> for the entity. The entry provides
    ///     access to change tracking information and operations for the entity.
    /// </returns>
    public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
    {
        SetAsModifiedEntity(entity);

        return base.Update(entity);
    }

    /// <summary>
    ///     Begins tracking the given entities and entries reachable from the given entities using
    ///     the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Modified" /> state by default, but see below for cases
    ///     when a different state will be used.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    public override void UpdateRange(params object[] entities)
    {
        for (long i = 0; i < entities.LongLength; i++)
        {
            var entity = entities[i];

            SetAsModifiedEntity(entity);
        }

        base.UpdateRange(entities);
    }

    /// <summary>
    ///     Begins tracking the given entities and entries reachable from the given entities using
    ///     the <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Modified" /> state by default, but see below for cases
    ///     when a different state will be used.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    public override void UpdateRange(IEnumerable<object> entities)
    {
        UpdateRange(entities.ToArray());
    }

    #endregion

    #region Remove

    /// <summary>
    ///     Removes specified <paramref name="entity"/> using specified <paramref name="removeType"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="entity">The entity to remove.</param>
    /// <param name="removeType">Remove type.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    public async ValueTask RemoveAsync<TEntity>(TEntity entity, RemoveType removeType = RemoveType.ViaChangesTracker, CancellationToken cancellationToken = default) where TEntity : Entity
    {
        switch (removeType)
        {
            case RemoveType.ViaChangesTracker:
                Remove(entity);
                break;

            case RemoveType.SoftViaChangesTracker:
                entity.SetAsDeleted();
                Update(entity);
                break;

            case RemoveType.SoftImmediate:
                await Set<TEntity>()
                    .Where(e => e.ID == entity.ID)
                    .ExecuteUpdateAsync(e => e
                        .SetProperty(e => e.LifecycleState, e => LifecycleState.Deleted)
                        .SetProperty(e => e.DeleteTS, e => DateTime.UtcNow),
                    cancellationToken);
                break;

            case RemoveType.Force:
                await Set<TEntity>()
                    .Where(e => e.ID == entity.ID)
                    .ExecuteDeleteAsync(cancellationToken);
                break;

            default:
                break;
        }
    }

    /// <summary>
    ///     Removes all entities based on an <paramref name="expression"/> using specified <paramref name="removeType"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="expression">A function to test each element for a condition.</param>
    /// <param name="removeType">Remove type.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    public async ValueTask RemoveAllAsync<TEntity>(Expression<Func<TEntity, bool>> expression, RemoveType removeType = RemoveType.ViaChangesTracker, CancellationToken cancellationToken = default) where TEntity : Entity
    {
        var entities = Set<TEntity>().Where(expression);

        switch (removeType)
        {
            case RemoveType.ViaChangesTracker:
                RemoveRange(entities);
                break;

            case RemoveType.SoftViaChangesTracker:
                var entitiesAsArray = await LinqExtensions.ToArrayAsync(entities, cancellationToken);

                for (long i = 0; i < entitiesAsArray.LongLength; i++)
                {
                    var entity = entitiesAsArray[i];

                    entity.SetAsDeleted();
                    Update(entity);
                }
                break;

            case RemoveType.SoftImmediate:
                await entities
                    .ExecuteUpdateAsync(e => e
                        .SetProperty(e => e.LifecycleState, e => LifecycleState.Deleted)
                        .SetProperty(e => e.DeleteTS, e => DateTime.UtcNow),
                    cancellationToken);
                break;

            case RemoveType.Force:
                await entities.ExecuteDeleteAsync(cancellationToken);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Save changes

    /// <summary>
    ///     Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     A task that represents the asynchronous save operation.
    ///     The task result contains the number of state entries written to the database.
    /// </returns>
    /// <exception cref="DbUpdateException"></exception>
    /// <exception cref="ConcurrentModificationException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrentModificationException(ex.Message);
        }
    }

    #endregion

    #region Utils methods

    /// <summary>
    ///     Asynchronously determines whether or not the database is available and can be connected to
    /// </summary>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the database is available; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="OperationCanceledException" />
    public async ValueTask<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        return await Database.CanConnectAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously starts a new transaction
    /// </summary>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous transaction initialization
    /// </returns>
    /// <exception cref="OperationCanceledException" />
    public async ValueTask BeginTransationAsync(CancellationToken cancellationToken = default)
    {
        await Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    ///     Asynchronously commits all changes made to the database in the current transaction
    /// </summary>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation
    /// </returns>
    /// <exception cref="OperationCanceledException" />
    public async ValueTask CommitTransationAsync(CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction is not null)
        {
            await Database.CurrentTransaction.CommitAsync(cancellationToken);
        }
    }

    /// <summary>
    ///     Asynchronously discards all changes made to the database in the current transaction
    /// </summary>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation
    /// </returns>
    /// <exception cref="OperationCanceledException" />
    public async ValueTask RollbackTransationAsync(CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction is not null)
        {
            await Database.CurrentTransaction.RollbackAsync(cancellationToken);
        }
    }

    /// <summary>
    ///     Asynchronously discards all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation
    /// </returns>
    /// <exception cref="OperationCanceledException" />
    public async ValueTask RollbackChangesAsync(CancellationToken cancellationToken = default)
    {
        var changedEntries = ChangeTracker
            .Entries()
            .Where(x => x.State != EntityState.Unchanged)
            .ToArray();

        for (long i = 0; i < changedEntries.LongLength; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entry = changedEntries[i];

            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;

                default:
                    break;
            }
        }

        await ValueTask.CompletedTask;
    }

    #endregion

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DoubleGuid>().HaveConversion<DoubleGuidValueConverter>();

        var assemblies = new List<Assembly>
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(Entity))!,
            Assembly.GetAssembly(GetType())!
        };

        assemblies.AddRange(Options.AdditionalEnumerationsAssemblies);

        var enumerations = assemblies
            .SelectMany(a => a.GetTypes().Where(t => t.BaseType == typeof(Enumeration)))
            .ToArray();

        var converter = typeof(EnumerationDataContextValueConverter<>);

        for (long i = 0; i < enumerations.LongLength; i++)
        {
            var enumeration = enumerations[i];

            configurationBuilder.Properties(enumeration).HaveConversion(converter.MakeGenericType(enumeration));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        if (Options.LifecycleQueryFilterApplied)
        {
            ApplyLifecycleQueryFilter(modelBuilder);
        }

        if (Options.UseRequestsLogsTable)
        {
            var requestsLogsTableType = Options.RequestsLogsTableType!;

            //modelBuilder.Entity(requestsLogsTableType)
            //    .Property(nameof(RequestLog.Details))
            //    .HasColumnType("json");

            //modelBuilder.Entity(requestsLogsTableType)
            //    .Property(nameof(RequestLog.ErrorDetails))
            //    .HasColumnType("json");
        }
    }

    protected static void ApplyLifecycleQueryFilter(ModelBuilder modelBuilder)
    {
        Expression<Func<Entity, bool>> filterExpr = i => i.LifecycleState != LifecycleState.Deleted;

        var entities = modelBuilder.Model
            .GetEntityTypes()
            .Where(t => t.ClrType.IsAssignableTo(typeof(Entity)))
            .ToArray();

        for (long i = 0; i < entities.LongLength; i++)
        {
            var mutableEntityType = entities[i];

            var parameter = Expression.Parameter(mutableEntityType.ClrType);
            var body = ReplacingExpressionVisitor.Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);
            var lambdaExpression = Expression.Lambda(body, parameter);

            mutableEntityType.SetQueryFilter(lambdaExpression);
        }
    }

    private static void SetAsNewEntity(object entity)
    {
        if (entity is Entity tridentonEntity && tridentonEntity.ID == DoubleGuid.Empty)
        {
            tridentonEntity.ID = DoubleGuid.NewGuid();
        }
    }

    private static void SetAsModifiedEntity(object entity)
    {
        if (entity is Entity tridentonEntity)
        {
            tridentonEntity.SetAsModified();
        }
    }

    private void VerifyDatabaseCreated()
    {
        if (Options.IsDatabaseCreatedVerificationEnabled)
        {
            Database.EnsureCreated();
        }
    }
}

public enum RemoveType
{
    /// <summary>
    ///     Executes regular <b>remove</b> when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    ViaChangesTracker,

    /// <summary>
    ///     Sets <see cref="Entity.LifecycleState"/> to <see cref="LifecycleState.Deleted"/>,
    ///     writes <see cref="DateTime.UtcNow"/> to <see cref="Entity.DeleteTS"/>
    ///     and executes regular <b>update</b> when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" /> is called.
    /// </summary>
    SoftViaChangesTracker,

    /// <summary>
    ///     Sets <see cref="Entity.LifecycleState"/> to <see cref="LifecycleState.Deleted"/>,
    ///     writes <see cref="DateTime.UtcNow"/> to <see cref="Entity.DeleteTS"/>
    ///     and executes regular update <b>immediately</b>, without calling <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" />.
    /// </summary>
    SoftImmediate,

    /// <summary>
    ///     Removes entity <b>immediately</b>, without calling <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" />.
    /// </summary>
    Force,
}