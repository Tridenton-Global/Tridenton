namespace Tridenton.DDD;

public abstract class Entity : IEquatable<Entity>
{
    public Guid ID { get; init; }

    private List<DomainEvent> _events;

    public IReadOnlyCollection<DomainEvent> Events => _events.AsReadOnly();

    protected Entity(Guid id)
	{
        ID = id;
        _events = new();
	}

    public bool Equals(Entity? other)
    {
        return other is not null && other.GetType() == GetType() && other.ID == ID;
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity other ? Equals(other) : base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }

    protected void RaiseEvent(DomainEvent @event)
    {
        _events.Add(@event);
    }
}