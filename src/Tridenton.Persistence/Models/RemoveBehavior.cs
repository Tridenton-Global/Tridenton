namespace Tridenton.Persistence;

public enum RemoveBehavior
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