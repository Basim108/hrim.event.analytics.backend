using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Jobs;
using Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis.Models;
using Hrim.Event.Analytics.EfCore;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Analysis.Cqrs.GapAnalysis;

/// <summary> Initiate gap-analysis calculation </summary>
public record GapAnalysisRecurringJob(Guid CorrelationId): AnalyticsRecurringJob(CorrelationId);

public class GapAnalysisRecurringJobHandler: IRequestHandler<GapAnalysisRecurringJob>
{
    private readonly ILogger<GapAnalysisRecurringJobHandler> _logger;
    private readonly IMediator                               _mediator;
    private readonly EventAnalyticDbContext                  _context;

    public GapAnalysisRecurringJobHandler(ILogger<GapAnalysisRecurringJobHandler> logger,
                                          IMediator                               mediator,
                                          EventAnalyticDbContext                  context) {
        _logger   = logger;
        _mediator = mediator;
        _context  = context;
    }

    public async Task Handle(GapAnalysisRecurringJob request, CancellationToken cancellationToken) {
        var eventTypeList = await _mediator.Send(new GetEventTypesForAnalysis(FeatureCodes.GAP_ANALYSIS), cancellationToken);
        foreach (var info in eventTypeList) {
            using var eventTypeScope = _logger.BeginScope(CoreLogs.EVENT_TYPE_ID, info.EventTypeId);

            var startedAt   = DateTime.UtcNow.TruncateToMicroseconds();
            var gapSettings = new GapSettings(info.Settings!);
            var prevAnalysisResult = await _context.StatisticsForEventTypes
                                                   .FirstOrDefaultAsync(x => x.EntityId == info.EventTypeId && x.AnalysisCode == FeatureCodes.GAP_ANALYSIS,
                                                                        cancellationToken);
            var analysisResult = await _mediator.Send(new CalculateGapForEventType(info.EventTypeId, gapSettings, prevAnalysisResult),
                                                      cancellationToken);
            if (analysisResult != null) {
                var resultJson = analysisResult.EventCount == 0
                                     ? null
                                     : JsonConvert.SerializeObject(analysisResult);
                await _mediator.Send(new SaveEventTypeAnalysisResult(LoadedDbEntity: prevAnalysisResult,
                                                                     info.EventTypeId,
                                                                     FeatureCodes.GAP_ANALYSIS,
                                                                     resultJson,
                                                                     startedAt,
                                                                     FinishedAt: DateTime.UtcNow.TruncateToMicroseconds(),
                                                                     request.CorrelationId),
                                     cancellationToken);
            }
        }
    }
}