using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Api.Middleware;
#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Services; 

public interface IApiRequestAccessor
{
    string GetStringCorrelationId();

    Guid GetCorrelationId();
    
    OperationContext GetOperationContext();
}

public class ApiRequestAccessor: IApiRequestAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiRequestAccessor(IHttpContextAccessor httpContextAccessor) { _httpContextAccessor = httpContextAccessor; }
    
    public string GetStringCorrelationId() =>
        _httpContextAccessor.HttpContext?.Response.Headers[CorrelationMiddleware.CORRELATION_ID_HEADER] 
     ?? Guid.Empty.ToString();
    
    public Guid             GetCorrelationId()    => Guid.Parse(GetStringCorrelationId());
    public OperationContext GetOperationContext() => new OperationContext(Guid.Parse("2c469d37-3728-4d1a-974a-6702cbdf3642"),
                                                                          GetCorrelationId());
}