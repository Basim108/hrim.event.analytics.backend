using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;

/// <summary>
/// Create or update a list of analysis settings for a particular event-type
/// </summary>
public record UpdateAnalysisForEventType(long EventTypeId, List<AnalysisConfigByEventType> Analysis, OperationContext Context)
    : OperationRequest(Context: Context), IRequest<CqrsResult<List<AnalysisConfigByEventType>?>>;
