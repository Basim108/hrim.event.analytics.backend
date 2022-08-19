using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

public class GetViewEventTypesHandler: IRequestHandler<GetAllViewEventTypes, IList<ViewSystemEventType>> {
    private readonly IMediator _mediator;

    public GetViewEventTypesHandler(IMediator mediator) {
        _mediator = mediator;
    }

    public async Task<IList<ViewSystemEventType>> Handle(GetAllViewEventTypes request, CancellationToken cancellationToken) {
        var durationTypes = await _mediator.Send(new GetViewDurationEventTypes(request.CorrelationId,
                                                                               request.IsPublic,
                                                                               request.IsDeleted,
                                                                               request.CreatedById),
                                                 cancellationToken);
        var occurrenceTypes = await _mediator.Send(new GetViewOccurrenceEventTypes(request.CorrelationId,
                                                                                   request.IsPublic,
                                                                                   request.IsDeleted,
                                                                                   request.CreatedById),
                                                   cancellationToken);
        var result = new List<ViewSystemEventType>(durationTypes.Count + occurrenceTypes.Count);
        result.AddRange(durationTypes);
        result.AddRange(occurrenceTypes);
        return result;
    }
}