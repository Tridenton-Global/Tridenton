using System.Text.Encodings.Web;

namespace Tridenton.Core.Util;

public readonly struct Serializer
{
    private static JsonSerializerOptions GetOptions(bool writeIndented = true)
    {
        return new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = writeIndented,
        };
    }

    public static string ToJson<TEntity>(TEntity entity, bool writeIndented = true)
    {
        if (entity is null) return string.Empty;

        return JsonSerializer.Serialize(entity, GetOptions(writeIndented));
    }

    public static TEntity? FromJson<TEntity>(string json, bool writeIndented = true, TEntity defaultValue = default!)
    {
        return JsonSerializer.Deserialize<TEntity>(json, GetOptions(writeIndented)) ?? defaultValue;
    }

    public static TEntity? FromJson<TEntity>(Type type, string json, bool writeIndented = true, TEntity defaultValue = default!)
    {
        return (TEntity)JsonSerializer.Deserialize(JsonDocument.Parse(json), type, GetOptions(writeIndented))! ?? defaultValue;
    }
}