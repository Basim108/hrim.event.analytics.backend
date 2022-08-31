using Hrim.Event.Analytics.Api.Middleware;

namespace Hrim.Event.Analytics.Api.Extensions; 

/// <summary> Extensions to register custom middlewares </summary>
public static class MiddlewareExtensions {
    /// <summary>
    /// Register <see cref="CorrelationMiddleware"/> so correlation id header will be processed
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        => builder.UseMiddleware<CorrelationMiddleware>();    
    
    /// <summary>
    /// Logging unhandled exceptions and http request & response headers & body.
    /// </summary>
    public static IApplicationBuilder UseHttpContextLogging(this IApplicationBuilder builder)
        => builder.UseMiddleware<HttpContextLoggingMiddleware>();
}