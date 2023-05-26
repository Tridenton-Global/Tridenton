namespace Tridenton.CQRS;

public abstract class NotificationHandler<TNotification> : BaseHandler where TNotification : TridentonRequest
{
    protected NotificationHandler(IServiceProvider services) : base(services) { }

    public abstract ValueTask HandleAsync(INotificationContext<TNotification> context);
}