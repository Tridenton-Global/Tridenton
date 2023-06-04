namespace Tridenton.DDD;

public abstract class DomainEvent : TridentonRequest
{
    Guid Id { get; }

    DateTime OccurredOn { get; }

    public DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}