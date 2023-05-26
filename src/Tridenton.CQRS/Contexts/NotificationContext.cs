namespace Tridenton.CQRS;

public interface INotificationContext<TNotification> : IContextBase where TNotification : TridentonRequest
{
    TNotification Notification { get; }
}

internal sealed record NotificationContext<TNotification>(TNotification Notification, CancellationToken CancellationToken) : INotificationContext<TNotification> where TNotification : TridentonRequest;