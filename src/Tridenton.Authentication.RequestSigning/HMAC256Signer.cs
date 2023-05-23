using System.Security.Cryptography;

namespace Tridenton.Authentication.RequestSigning;

internal sealed class HMAC256Signer : RequestSigner
{
    public HMAC256Signer() : base(SigningAlgorithm.HmacSHA256) { }

    public override async ValueTask<string> ComputeHashAsync(string content, CancellationToken cancellationToken = default)
    {
        using var sha256 = SHA256.Create();

        var hashedBytes = await sha256.ComputeHashAsync(new MemoryStream(Encoding.UTF8.GetBytes(content)), cancellationToken);
        return hashedBytes.ToBase64().ToString();
    }

    protected override KeyedHashAlgorithm CreateAlgorithm(ReadOnlySpan<byte> key) => new HMACSHA256(key.ToArray());
}