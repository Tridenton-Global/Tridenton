namespace Tridenton.Core.Util.Converters;

internal sealed class DoubleGuidJsonConverter : JsonConverter<DoubleGuid>
{
    public override DoubleGuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        return DoubleGuid.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, DoubleGuid value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}