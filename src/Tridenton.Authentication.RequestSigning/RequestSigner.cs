using Microsoft.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web;
using Tridenton.Extensions.Http;

namespace Tridenton.Authentication.RequestSigning;

internal abstract class RequestSigner : IRequestSigner
{
    public readonly SigningAlgorithm Algorithm;

    protected RequestSigner(SigningAlgorithm algorithm)
    {
        Algorithm = algorithm;
    }

    public ValueTask SignRequestAsync(HttpRequestMessage request, SigningCredentials credentials, CancellationToken cancellationToken = default)
    {
        return SignRequestAsync(request, credentials.AccessKey, credentials.SecretKey, cancellationToken);
    }

    public async ValueTask SignRequestAsync(HttpRequestMessage request, string accessKey, string secretKey, CancellationToken cancellationToken = default)
    {
        var canonicalRequest = await GetCanonicalRequestAsync(request, cancellationToken);
        var stringToSign = await CreateStringToSignAsync(canonicalRequest, cancellationToken);
        var signature = await ComputeSignatureAsync(stringToSign, secretKey, cancellationToken);

        request.Headers.Add(HeaderNames.Authorization, $"{Algorithm} {RequestSigningConstants.Credential}={accessKey}, {RequestSigningConstants.Signature}={signature}");
    }

    private async ValueTask<string> GetCanonicalRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(request.Method.Method);
        stringBuilder.AppendLine(request.RequestUri!.AbsolutePath);
        stringBuilder.AppendLine(HttpUtility.ParseQueryString(request.RequestUri.Query).GetCanonicalQueryParameters().ToString());
        stringBuilder.AppendLine(GetCanonicalHeaders(request, out var signedHeaders).ToString());
        stringBuilder.AppendLine(string.Join(";", signedHeaders));
        stringBuilder.Append(await GetPayloadHash(request, cancellationToken));

        return stringBuilder.ToString();
    }

    private static ReadOnlySpan<char> GetCanonicalHeaders(HttpRequestMessage request, out string[] signedHeaders)
    {
        signedHeaders = Array.Empty<string>();

        var allHeaders = request.Headers.ToDictionary(x => x.Key, x => string.Join(";", x.Value));

        if (request.Content is not null)
        {
            foreach (var contentHeader in request.Content.Headers)
            {
                allHeaders.Add(contentHeader.Key, string.Join(";", contentHeader.Value));
            }
        }

        if (!allHeaders.ContainsKey(HeaderNames.Host))
        {
            allHeaders.Add(HeaderNames.Host, request.RequestUri!.Host);
        }

        return allHeaders.GetCanonicalHeaders(out signedHeaders);
    }

    private async ValueTask<string> GetPayloadHash(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var payload = request.Content is not null ? await request.Content.ReadAsStringAsync(cancellationToken) : string.Empty;

        var payloadHash = await ComputeHashAsync(payload, cancellationToken);

        return payloadHash.ToHexString().ToString();
    }

    public virtual async ValueTask<string> CreateStringToSignAsync(string canonicalRequest, CancellationToken cancellationToken = default)
    {
        var hashedCanonicalRequest = await ComputeHashAsync(canonicalRequest, cancellationToken);

        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(Algorithm);
        stringBuilder.AppendLine(hashedCanonicalRequest.ToHexString().ToString());

        return stringBuilder.ToString();
    }

    public virtual async ValueTask<string> ComputeSignatureAsync(string stringToSign, string secretKey, CancellationToken cancellationToken = default)
    {
        using var algorithm = CreateAlgorithm(secretKey.BytesFromBase64());

        var bytes = Encoding.ASCII.GetBytes(stringToSign);

        var hashedBytes = await algorithm.ComputeHashAsync(new MemoryStream(bytes), cancellationToken);

        return hashedBytes.ToBase64().ToHexString().ToString();
    }

    public abstract ValueTask<string> ComputeHashAsync(string content, CancellationToken cancellationToken = default);

    protected abstract KeyedHashAlgorithm CreateAlgorithm(ReadOnlySpan<byte> key);
}