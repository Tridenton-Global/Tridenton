namespace Tridenton.Persistence;

public abstract class Entity : Entity<DoubleGuid> { }

public abstract class Entity<TId> where TId : struct
{
    [Key]
    public TId ID { get; internal set; }

    public LifecycleState LifecycleState { get; private set; }

    public DateTime CreateTS { get; private set; }

    public DateTime UpdateTS { get; private set; }

    public DateTime? DeleteTS { get; internal set; }

    public Entity()
    {
        ID = default;

        LifecycleState = LifecycleState.Active;

        CreateTS = UpdateTS = DateTime.UtcNow;
        DeleteTS = null;
    }

    internal void SetAsModified()
    {
        UpdateTS = DateTime.UtcNow;
    }

    internal void SetAsDeleted()
    {
        DeleteTS = DateTime.UtcNow;
        LifecycleState = LifecycleState.Deleted;
    }
}

[TypeConverter(typeof(EnumerationTypeConverter<LifecycleState>))]
[JsonConverter(typeof(EnumerationJsonConverter<LifecycleState>))]
public class LifecycleState : Enumeration
{
    protected LifecycleState(string value) : base(value) { }

    public static readonly LifecycleState Active = new("Active");
    public static readonly LifecycleState Deleted = new("Deleted");
}