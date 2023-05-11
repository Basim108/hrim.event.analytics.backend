using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Middleware;

/// <summary>
///     Logs http request headers and body and unhandled exceptions
/// </summary>
[ExcludeFromCodeCoverage]
public class HttpContextLoggingMiddleware
{
    private const    int                                   MAX_BODY_LENGTH = 32_000;
    private readonly bool                                  _isDevelopment;
    private readonly bool                                  _isStaging;
    private readonly ILogger<HttpContextLoggingMiddleware> _logger;

    private readonly RequestDelegate _next;

    public HttpContextLoggingMiddleware(RequestDelegate                       next,
                                        IWebHostEnvironment                   environment,
                                        ILogger<HttpContextLoggingMiddleware> logger) {
        _next          = next;
        _isDevelopment = environment.IsDevelopment();
        _isStaging     = environment.IsStaging();
        _logger        = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        var isRequestSkipped = true;
        if (_isDevelopment || _isStaging) {
            await LogRequestAsync(context: context);
            isRequestSkipped = false;
        }

        await _next(context: context);

        using var responseStatusScope = _logger.BeginScope(messageFormat: ApiLogs.RESPONSE_STATUS_CODE, context.Response.StatusCode);
        var       isError             = context.Response.StatusCode >= 500;
        if (isError && isRequestSkipped) await LogRequestAsync(context: context);
        if (isError) LogError(context: context);
    }

    private async Task LogRequestAsync(HttpContext context) {
        var requestHeaders = JsonConvert.SerializeObject(value: context.Request.Headers);
        _logger.LogInformation(message: ApiLogs.REQUEST_HEADERS, requestHeaders);

        var requestBody = await GetBodyAsync(context: context);
        if (requestBody.Length > MAX_BODY_LENGTH)
            requestBody = requestBody.Substring(startIndex: 0, length: MAX_BODY_LENGTH);
        _logger.LogInformation(message: ApiLogs.REQUEST_BODY, requestBody);
    }

    private void LogError(HttpContext context) {
        var ex = context.Features
                        .Get<IExceptionHandlerPathFeature>()
                       ?.Error;
        if (ex != null)
            _logger.LogError(message: ApiLogs.UNHANDLED_EXCEPTION, ex.ToString());
    }

    private static async Task<string> GetBodyAsync(HttpContext context) {
        context.Request.EnableBuffering();
        context.Request.Body.Position = 0;
        var requestBody = await new StreamReader(stream: context.Request.Body).ReadToEndAsync()
                                                                              .ConfigureAwait(continueOnCapturedContext: false);
        context.Request.Body.Position = 0;
        return requestBody;
    }
}