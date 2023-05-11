using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Middleware;

/// <summary>
///     Adds/takes correlation id to/from headers and create a logging scope with that id.
/// </summary>
[ExcludeFromCodeCoverage]
public class CorrelationMiddleware
{
    public const     string                         CORRELATION_ID_HEADER = "X-Correlation-ID";
    private readonly ILogger<CorrelationMiddleware> _logger;

    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate                next,
                                 ILogger<CorrelationMiddleware> logger) {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        var correlationHeader = context.Request.Headers[key: CORRELATION_ID_HEADER];
        var correlationId     = GetCorrelationId(header: correlationHeader);

        using var correlationScope = _logger.BeginScope(messageFormat: "CorrelationId={CorrelationId}", correlationId);
        if (context.Response.Headers.ContainsKey(key: CORRELATION_ID_HEADER))
            context.Response.Headers[key: CORRELATION_ID_HEADER] = correlationId.ToString();
        else
            context.Response.Headers.Add(key: CORRELATION_ID_HEADER, correlationId.ToString());
        await _next(context: context);
    }

    private static Guid GetCorrelationId(string? header) {
        return string.IsNullOrWhiteSpace(value: header)
                   ? Guid.NewGuid()
                   : Guid.Parse(input: header);
    }
}