using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Jobs;
using Hrim.Event.Analytics.JobWorker.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.JobWorker.MediatR;

[ExcludeFromCodeCoverage]
    public class MediatorHangfireBridge
    {
        private readonly IMediator                       _mediator;
        private readonly ILogger<MediatorHangfireBridge> _logger;

        public MediatorHangfireBridge(IMediator      mediator,
                                      ILoggerFactory loggerFactory)
        {
            _mediator = mediator;
            _logger   = loggerFactory.CreateLogger<MediatorHangfireBridge>();
        }

        [DisplayName("MediatorHangfireBridge.Send: {0}")]
        public async Task SendAsync<T>(T request) where T : IRequest, IAnalyticsJob
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var correlationScope = string.IsNullOrWhiteSpace(request.CorrelationId)
                                       ? _logger.BeginScope(CoreLogs.CORRELATION_ID, Guid.NewGuid())
                                       : _logger.BeginScope(CoreLogs.CORRELATION_ID, request.CorrelationId);
            try {
                try {
                    await _mediator.Send(request);
                }
                catch (Exception ex) {
                    var cqrsCommandName = typeof(T).Name;
                    _logger.LogError(ex, JobLogs.JOB_FAILED_WITH_ERROR, cqrsCommandName);
                    throw new AnalyticsJobException(cqrsCommandName, JobLogs.JOB_FAILED_WITH_ERROR, ex);
                }
            }
            finally {
                correlationScope?.Dispose();
            }
        }
    }