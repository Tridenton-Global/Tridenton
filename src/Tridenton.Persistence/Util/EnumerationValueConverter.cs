namespace Tridenton.Persistence;

internal sealed class EnumerationDataContextValueConverter<TEnum> : DataContextValueConverter<TEnum, string> where TEnum : Enumeration
{
    public EnumerationDataContextValueConverter() : base(e => e.Value, e => Enumeration.GetValue<TEnum>(e)!) { }
}