using System.Globalization;

namespace Tridenton.Core.Util.Converters;

public sealed class EnumerationJsonConverter<TEnumeration> : JsonConverter<TEnumeration> where TEnumeration : Enumeration
{
    public override TEnumeration? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        return Enumeration.GetValue<TEnumeration>(value!);
    }

    public override void Write(Utf8JsonWriter writer, TEnumeration value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}

public sealed class EnumerationTypeConverter<TEnumeration> : TypeConverter where TEnumeration : Enumeration
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || sourceType == typeof(TEnumeration);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
    {
        return Enumeration.GetValue<TEnumeration>(value!.ToString()!);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        return value is TEnumeration constant ? constant.Value : base.ConvertTo(context, culture, value, destinationType);
    }
}