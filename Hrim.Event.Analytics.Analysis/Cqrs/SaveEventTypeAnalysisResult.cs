using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.EfCore;
using MediatR;

namespace Hrim.Event.Analytics.Analysis.Cqrs;

/// <summary>
/// Command to save an analysis calculation result.
/// Is called from recurring analysis jobs. 
/// </summary>
public record SaveEventTypeAnalysisResult(StatisticsForEventType? LoadedDbEntity,
                                          Guid                    EventTypeId,
                                          string                  AnalysisCode,
                                          string?                 ResultJson,
                                          DateTime                StartedAt,
                                          DateTime                FinishedAt,
                                          Guid                    CorrelationId): IRequest;

public class SaveEventTypeAnalysisResultHandler: IRequestHandler<SaveEventTypeAnalysisResult>
{
    private readonly EventAnalyticDbContext _context;

    public SaveEventTypeAnalysisResultHandler(EventAnalyticDbContext context) { _context = context; }

    public Task Handle(SaveEventTypeAnalysisResult request, CancellationToken cancellationToken) {
        if (request.EventTypeId == Guid.Empty)
            throw new ArgumentNullException(nameof(request), nameof(request.EventTypeId));
        if (string.IsNullOrWhiteSpace(request.AnalysisCode))
            throw new ArgumentNullException(nameof(request), nameof(request.AnalysisCode));
        if (request.StartedAt == DateTime.MinValue)
            throw new ArgumentNullException(nameof(request), nameof(request.StartedAt));
        if (request.FinishedAt == DateTime.MinValue)
            throw new ArgumentNullException(nameof(request), nameof(request.FinishedAt));
        if (request.StartedAt > request.FinishedAt)
            throw new ArgumentNullException(nameof(request), AnalysisLogs.STARTING_AT_IS_GREATER_THAN_FINISHED_AT);

        return HandleAsync(request, cancellationToken);
    }

    private async Task HandleAsync(SaveEventTypeAnalysisResult request, CancellationToken cancellationToken) {
        var entity = request.LoadedDbEntity ?? new StatisticsForEventType();
        entity.EntityId      = request.EventTypeId;
        entity.AnalysisCode  = request.AnalysisCode;
        entity.StartedAt     = request.StartedAt;
        entity.FinishedAt    = request.FinishedAt;
        entity.CorrelationId = request.CorrelationId;
        entity.ResultJson    = request.ResultJson;

        if (request.LoadedDbEntity == null) {
            _context.StatisticsForEventTypes.Add(entity);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}