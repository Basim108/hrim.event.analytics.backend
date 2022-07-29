using Hrim.Event.Analytics.Api.Middleware;

namespace Hrim.Event.Analytics.Api.Extensions; 

/// <summary> </summary>
public static class MiddlewareExtensions {
    /// <summary>
    /// Register <see cref="CorrelationMiddleware"/> so correlation id header will be processed
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        => builder.UseMiddleware<CorrelationMiddleware>();    
}