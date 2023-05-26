namespace Tridenton.CQRS;

public interface IContextBase : ICancelable
{
    DoubleGuid ID => DoubleGuid.NewGuid();

    DateTime EventTS => DateTime.UtcNow;
}