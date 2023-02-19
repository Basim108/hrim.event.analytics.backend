using Hrim.Event.Analytics.Api.Middleware;

namespace Hrim.Event.Analytics.Api.Extensions;
#pragma warning disable CS1591, CS1570

public static class MiddlewareExtensions
{
    /// <summary>
    ///     Register <see cref="CorrelationMiddleware" /> so correlation id header will be processed
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationMiddleware>();
    }

    /// <summary> Logging unhandled exceptions and http request & response headers & body. </summary>
    public static IApplicationBuilder UseHttpContextLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HttpContextLoggingMiddleware>();
    }
}