using System.ComponentModel.DataAnnotations;
using System.Net;
using Tridenton.Core.Models;
using Tridenton.Core.Util;

namespace Tridenton.CQRSPersistenceInteraction;

public class RequestLog
{
    [Key]
    public DoubleGuid ID { get; set; }

    public string Request { get; set; }

    public string Details { get; set; }

    public string RemoteAddress { get; set; }

    public DoubleGuid ResponseID { get; set; }

    public ResponseStatus ResponseStatus { get; set; }

    public HttpStatusCode HttpStatus { get; set; }

    public string? ErrorDetails { get; set; }

    public DateTime RequestTS { get; set; }

    public TimeSpan ElapsedTime { get; set; }

    public long AllocatedMegabytes { get; set; }

    public RequestLog()
    {
        Request = Details = RemoteAddress = string.Empty;

        ResponseStatus = ResponseStatus.Error;
        HttpStatus = HttpStatusCode.BadRequest;
    }
}

public class RequestLogError
{
    public ErrorSide? ErrorSide { get; set; }

    public string Message { get; set; }

    public string StackTrace { get; set; }

    public RequestLogError()
    {
        Message = StackTrace = string.Empty;
    }
}