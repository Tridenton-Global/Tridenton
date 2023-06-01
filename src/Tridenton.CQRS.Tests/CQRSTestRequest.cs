using Tridenton.Core.Models;

namespace Tridenton.CQRS.Tests;

//public class CQRSTestRequest : TridentonRequest
//{
//	public int DesirableValue { get; set; }

//	public CQRSTestRequest()
//	{
//        DesirableValue = 54;
//	}
//}

//internal sealed class CQRSTestRequestHandler : RequestHandler<CQRSTestRequest>
//{
//    private readonly ITransientService _transientService;

//    public CQRSTestRequestHandler(IServiceProvider services) : base(services)
//    {
//        _transientService = GetService<ITransientService>();
//    }

//    public override async ValueTask HandleAsync(IRequestContext<CQRSTestRequest> context)
//    {
//        var request = context.Request;

//        var realValue = _transientService.GetValue();

//        if (realValue != request.DesirableValue) throw new Exception("Values do not match");

//        await ValueTask.CompletedTask;
//    }
//}