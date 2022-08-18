using Hrim.Event.Analytics.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.Infrastructure.Mediator;

public class PrePostLogWrapper<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse> {
    private readonly ILogger _logger;

    public PrePostLogWrapper(ILogger logger) {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest                          request,
                                        CancellationToken                 cancellationToken,
                                        RequestHandlerDelegate<TResponse> next) {
        using var commandScope = _logger.BeginScope(CoreLogs.CqrsCommand, typeof(TRequest).Name);
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(CoreLogs.StartHandling);

        var response = await next().ConfigureAwait(false);

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(CoreLogs.FinishHandling);
        return response;
    }
}