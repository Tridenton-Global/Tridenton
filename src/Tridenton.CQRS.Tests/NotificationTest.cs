using Tridenton.Core.Models;

namespace Tridenton.CQRS.Tests;

public class NotificationTest : TridentonRequest
{
    public int Value { get; }

    public NotificationTest()
    {
        Value = Random.Shared.Next();
    }
}

internal sealed class NotificationTestHandler1 : NotificationHandler<NotificationTest>
{
    public NotificationTestHandler1(IServiceProvider services) : base(services)
    {
    }

    public override async Task HandleAsync(INotificationContext<NotificationTest> context)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(context.Notification.Value);

        await Task.Delay(1_000);

        Console.WriteLine("Done 1");
    }
}

internal sealed class NotificationTestHandler2 : NotificationHandler<NotificationTest>
{
    public NotificationTestHandler2(IServiceProvider services) : base(services)
    {
    }

    public override async Task HandleAsync(INotificationContext<NotificationTest> context)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(context.Notification.Value);

        await Task.Delay(1_000);

        Console.WriteLine("Done 2");
    }
}

internal sealed class NotificationTestHandler3 : NotificationHandler<NotificationTest>
{
    public NotificationTestHandler3(IServiceProvider services) : base(services)
    {
    }

    public override async Task HandleAsync(INotificationContext<NotificationTest> context)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(context.Notification.Value);

        await Task.Delay(1_000);

        Console.WriteLine("Done 3");
    }
}
internal sealed class NotificationTestHandler4 : NotificationHandler<NotificationTest>
{
    public NotificationTestHandler4(IServiceProvider services) : base(services)
    {
    }

    public override async Task HandleAsync(INotificationContext<NotificationTest> context)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(context.Notification.Value);

        await Task.Delay(1_000);

        Console.WriteLine("Done 4");
    }
}