using Hrim.Event.Analytics.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.Infrastructure.Mediator;

public class PrePostLogWrapper<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PrePostLogWrapper<TRequest, TResponse>> _logger;

    public PrePostLogWrapper(ILogger<PrePostLogWrapper<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using var commandScope = _logger.BeginScope(CoreLogs.CQRS_COMMAND, typeof(TRequest).Name);
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(CoreLogs.START_HANDLING);

        var response = await next().ConfigureAwait(false);

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(CoreLogs.FINISH_HANDLING);
        return response;
    }
}