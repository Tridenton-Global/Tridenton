namespace Tridenton.Authentication.RequestSigning;

public record RequestSignature(SigningAlgorithm Algorithm, string Credential, string Signature)
{
    public override string ToString() => $"Algorithm: {Algorithm}; credentials: {Credential}; signature: {Signature}";

    public static readonly RequestSignature Empty = new(SigningAlgorithm.HmacSHA256, string.Empty, string.Empty);
}