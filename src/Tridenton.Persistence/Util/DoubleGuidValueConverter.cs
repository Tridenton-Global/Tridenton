namespace Tridenton.Persistence;

internal sealed class DoubleGuidValueConverter : DataContextValueConverter<DoubleGuid, string>
{
    public DoubleGuidValueConverter() : base(x => x.ToString(), x => DoubleGuid.Parse(x)) { }
}