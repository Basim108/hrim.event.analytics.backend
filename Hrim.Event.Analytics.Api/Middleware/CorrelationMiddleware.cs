using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Middleware;

/// <summary>
///     Adds/takes correlation id to/from headers and create a logging scope with that id.
/// </summary>
[ExcludeFromCodeCoverage]
public class CorrelationMiddleware
{
    public const string CORRELATION_ID_HEADER = "X-Correlation-ID";

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

        using var correlationScope = _logger.BeginScope(CoreLogs.CORRELATION_ID, correlationId);
        context.Response.Headers[CORRELATION_ID_HEADER] = correlationId.ToString();
        await _next(context: context);
    }

    private Guid GetCorrelationId(string? header) {
        try {
            return string.IsNullOrWhiteSpace(value: header)
                       ? Guid.NewGuid()
                       : Guid.Parse(input: header);
        }
        catch (FormatException ex) {
            _logger.LogWarning(ex, "Failed to parse correlation id header");
            return Guid.NewGuid();
        }
    }
}