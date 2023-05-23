namespace Tridenton.Authentication.RequestSigning;

/// <summary>
///     
/// </summary>
public interface IRequestSigner
{
    /// <summary>
    ///     
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <param name="credentials">Signing credentials</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    ValueTask SignRequestAsync(HttpRequestMessage request, SigningCredentials credentials, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="accessKey"></param>
    /// <param name="secretKey"></param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    ValueTask SignRequestAsync(HttpRequestMessage request, string accessKey, string secretKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="canonicalRequest"></param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    ValueTask<string> CreateStringToSignAsync(string canonicalRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stringToSign"></param>
    /// <param name="secretKey"></param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     
    /// </returns>
    ValueTask<string> ComputeSignatureAsync(string stringToSign, string secretKey, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Computes hash for specified <paramref name="content"/>
    /// </summary>
    /// <param name="content">String content to be hashed</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a hashed string
    /// </returns>
    ValueTask<string> ComputeHashAsync(string content, CancellationToken cancellationToken = default);
}