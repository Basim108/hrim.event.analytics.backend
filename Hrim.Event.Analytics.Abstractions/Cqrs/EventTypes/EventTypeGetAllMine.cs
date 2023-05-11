using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;

/// <summary> Returns a union of event types </summary>
public record EventTypeGetAllMine(OperationContext Context,
                                  bool             IncludeOthersPublic = false,
                                  bool             IncludeDeleted      = false)
    : OperationRequest(Context: Context), IRequest<IList<ViewEventType>>;