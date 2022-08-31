using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Middleware; 

/// <summary>
/// Adds/takes correlation id to/from headers and create a logging scope with that id. 
/// </summary>
[ExcludeFromCodeCoverage]
public class CorrelationMiddleware
{
    private readonly ILogger<CorrelationMiddleware> _logger;

    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate                next,
                                 ILogger<CorrelationMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public const string CORRELATION_ID_HEADER = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationHeader = context.Request.Headers[CORRELATION_ID_HEADER];
        var correlationId     = GetCorrelationId(correlationHeader);

        using var correlationScope = _logger.BeginScope("CorrelationId={CorrelationId}", correlationId);
        if(context.Response.Headers.ContainsKey(CORRELATION_ID_HEADER))
            context.Response.Headers[CORRELATION_ID_HEADER] = correlationId.ToString();
        else
            context.Response.Headers.Add(CORRELATION_ID_HEADER, correlationId.ToString());
        await _next(context);
    }

    private static Guid GetCorrelationId(string header) =>
        string.IsNullOrWhiteSpace(header)
            ? Guid.NewGuid()
            : Guid.Parse(header);
}