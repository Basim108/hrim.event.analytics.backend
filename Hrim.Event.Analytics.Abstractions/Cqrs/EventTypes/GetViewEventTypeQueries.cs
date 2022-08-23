using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

/// <summary> Returns a union of event types </summary>
public record GetViewEventTypes(OperationContext Context, bool IsPublic = true, bool IncludeDeleted = false)
    : OperationRequest(Context), IRequest<IList<ViewEventType>>;