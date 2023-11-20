using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;

/// <summary>
/// Get a list of analysis settings for a particular event-type
/// </summary>
public record GetAnalysisByEventTypeId(long EventTypeId, OperationContext Context)
    : OperationRequest(Context: Context), IRequest<CqrsResult<List<AnalysisConfigByEventType>?>>;
