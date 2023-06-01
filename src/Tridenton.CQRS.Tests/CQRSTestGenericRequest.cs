using Tridenton.Core.Models;

namespace Tridenton.CQRS.Tests;

//public class CQRSTestGenericRequest : TridentonRequest<CQRSTestGenericResponse>
//{
//	public int DesirableValue { get; set; }

//    public CQRSTestGenericRequest()
//    {
//        DesirableValue = 54;
//    }
//}

//public class CQRSTestGenericResponse : TridentonResponse
//{
//	public int Value { get; set; }
//}

//internal sealed class CQRSTestGenericRequestHandler : RequestHandler<CQRSTestGenericRequest, CQRSTestGenericResponse>
//{
//    private readonly ITransientService _transientService;

//    public CQRSTestGenericRequestHandler(IServiceProvider services) : base(services)
//    {
//        _transientService = GetService<ITransientService>();
//    }

//    public override async ValueTask<CQRSTestGenericResponse> HandleAsync(IRequestContext<CQRSTestGenericRequest, CQRSTestGenericResponse> context)
//    {
//        var request = context.Request;

//        var realValue = _transientService.GetValue();

//        return await ValueTask.FromResult(new CQRSTestGenericResponse
//        {
//            Value = realValue,
//        });
//    }
//}