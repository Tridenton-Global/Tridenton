namespace Tridenton.Core.Models;

/// <summary>
///     Base response model
/// </summary>
public abstract class TridentonResponse
{
    /// <summary>
    ///     Response status
    /// </summary>
    [JsonInclude]
    public ResponseStatus Status { get; set; }

    /// <summary>
    ///     HTTP status code
    /// </summary>
    [JsonInclude]
    public HttpStatusCode HttpStatus { get; set; }

    /// <summary>
    ///     Response metadata
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ResponseMetadata Metadata { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="TridentonResponse"/>
    /// </summary>
    [JsonConstructor]
    public TridentonResponse()
    {
        Status = ResponseStatus.Error;
        HttpStatus = HttpStatusCode.BadRequest;

        Metadata = new();
    }

    /// <summary>
    ///     Throws <see cref="FailedResponseException"/> with message from <see cref="Metadata"/> when <see cref="Status"/> equals to <see cref="ResponseStatus.Error"/>
    /// </summary>
    /// <exception cref="FailedResponseException"></exception>
    public void ThrowIfErrorStatus()
    {
        if (Status == ResponseStatus.Error)
        {
            if (Metadata != default)
            {
                throw new FailedResponseException(Metadata.Message!, HttpStatus, Metadata.ErrorSide!);
            }

            throw new FailedResponseException(ErrorMessage!, HttpStatus, ErrorSide.Sender);
        }
    }

    /// <summary>
    ///     Sets <see cref="Metadata"/>, <see cref="Status"/> and <see cref="HttpStatus"/> to <see langword="default"/>.
    ///     If <see cref="Metadata"/> has <see cref="ResponseMetadata.Message"/>, its value will be set into <see cref="ErrorMessage"/>
    /// </summary>
    public void HideMetadata()
    {
        ErrorMessage = Metadata.Message;
        Metadata = default!;
        Status = default!;
        HttpStatus = default;
    }
}

/// <summary>
///     Response metadata
/// </summary>
public class ResponseMetadata
{
    /// <summary>
    ///     Request identifier
    /// </summary>
    [JsonInclude]
    public DoubleGuid RequestID { get; set; }

    /// <summary>
    ///     Response identifier
    /// </summary>
    [JsonInclude]
    public DoubleGuid ResponseID { get; private set; }

    /// <summary>
    ///     Time stamp when request processing was started
    /// </summary>
    public DateTime RequestTS { get; set; }

    /// <summary>
    ///     Time stamp when response was ready
    /// </summary>
    public DateTime ResponseTS { get; set; }

    /// <summary>
    ///     Time interval of request processing
    /// </summary>
    public TimeSpan ProcessingInterval { get; set; }

    /// <summary>
    ///     Amount of bytes of request body
    /// </summary>
    public long RequestSize { get; set; }

    /// <summary>
    ///     Which end of a request was responsible for a service error response
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ErrorSide? ErrorSide { get; set; }

    /// <summary>
    ///     Response message
    /// </summary>
    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; }

    /// <summary>
    ///     Response metadata
    /// </summary>
    [JsonConstructor]
    public ResponseMetadata()
    {
        ResponseID = DoubleGuid.NewGuid();
        RequestID = DoubleGuid.Empty;
    }
}

/// <summary>
///     Base response options builder
/// </summary>
public abstract class ResponseOptionsBuilder
{
    /// <summary>
    ///     Defines whether response metadata should be hidden from JSON
    /// </summary>
    internal bool ResponseMetadataHidden { get; private set; }

    protected void HideResponseMetadata()
    {
        ResponseMetadataHidden = true;
    }
}

[TypeConverter(typeof(EnumerationTypeConverter<ResponseStatus>))]
[JsonConverter(typeof(EnumerationJsonConverter<ResponseStatus>))]
public sealed class ResponseStatus : Enumeration
{
    private ResponseStatus(string value) : base(value) { }

    public static readonly ResponseStatus Success = new("Success");
    public static readonly ResponseStatus Error = new("Error");
    public static readonly ResponseStatus Cancelled = new("Cancelled");
}

[TypeConverter(typeof(EnumerationTypeConverter<ErrorSide>))]
[JsonConverter(typeof(EnumerationJsonConverter<ErrorSide>))]
public sealed class ErrorSide : Enumeration
{
    private ErrorSide(string value) : base(value) { }

    public static readonly ErrorSide Sender = new("Sender");
    public static readonly ErrorSide Receiver = new("Receiver");
}