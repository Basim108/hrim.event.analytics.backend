using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

/// <summary> Returns a union of event types </summary>
//TODO: fix: it should be able to return both private and public event types.
public record EventTypeGetAllMine(OperationContext Context, bool IsPublic = true, bool IncludeDeleted = false)
    : OperationRequest(Context), IRequest<IList<ViewEventType>>;