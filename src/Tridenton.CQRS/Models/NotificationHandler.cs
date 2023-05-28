namespace Tridenton.CQRS;

public abstract class NotificationHandler<TNotification> : BaseHandler where TNotification : TridentonRequest
{
    protected NotificationHandler(IServiceProvider services) : base(services) { }

    public abstract Task HandleAsync(INotificationContext<TNotification> context);
}