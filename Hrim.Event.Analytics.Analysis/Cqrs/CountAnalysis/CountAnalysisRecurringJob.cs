using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.EfCore;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Analysis.Cqrs.CountAnalysis;

public class CountAnalysisRecurringJobHandler: IRequestHandler<CountAnalysisRecurringJob>
{
    private readonly ILogger<CountAnalysisRecurringJobHandler> _logger;
    private readonly IMediator                                 _mediator;
    private readonly EventAnalyticDbContext                    _context;

    public CountAnalysisRecurringJobHandler(ILogger<CountAnalysisRecurringJobHandler> logger,
                                            IMediator                               mediator,
                                            EventAnalyticDbContext                  context) {
        _logger   = logger;
        _mediator = mediator;
        _context  = context;
    }

    public async Task Handle(CountAnalysisRecurringJob request, CancellationToken cancellationToken) {
        var eventTypeList = await _mediator.Send(new GetEventTypesForAnalysis(FeatureCodes.COUNT_ANALYSIS), cancellationToken);
        foreach (var info in eventTypeList) {
            using var eventTypeScope = _logger.BeginScope(CoreLogs.EVENT_TYPE_ID, info.EventTypeId);

            var startedAt   = DateTime.UtcNow.TruncateToMicroseconds();
            var prevAnalysisResult = await _context.StatisticsForEventTypes
                                                   .FirstOrDefaultAsync(x => x.EntityId == info.EventTypeId && 
                                                                             x.AnalysisCode == FeatureCodes.COUNT_ANALYSIS,
                                                                        cancellationToken);
            var analysisResult = await _mediator.Send(new CalculateCountForEventType(info, prevAnalysisResult),
                                                      cancellationToken);
            if (analysisResult != null) {
                var resultJson = analysisResult is { DurationsCount: 0, OccurrencesCount : 0 }
                                     ? null
                                     : JsonConvert.SerializeObject(analysisResult);
                await _mediator.Send(new SaveEventTypeAnalysisResult(LoadedDbEntity: prevAnalysisResult,
                                                                     info.EventTypeId,
                                                                     FeatureCodes.COUNT_ANALYSIS,
                                                                     resultJson,
                                                                     startedAt,
                                                                     FinishedAt: DateTime.UtcNow.TruncateToMicroseconds(),
                                                                     request.CorrelationId),
                                     cancellationToken);
            }
        }
    }
}