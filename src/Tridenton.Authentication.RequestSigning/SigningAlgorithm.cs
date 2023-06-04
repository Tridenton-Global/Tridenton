namespace Tridenton.Authentication.RequestSigning;

[TypeConverter(typeof(EnumerationTypeConverter<SigningAlgorithm>))]
[JsonConverter(typeof(EnumerationJsonConverter<SigningAlgorithm>))]
public record SigningAlgorithm : Enumeration
{
    protected SigningAlgorithm(string value) : base(value) { }

    public static readonly SigningAlgorithm HmacSHA256 = new("HMAC-SHA256");
}