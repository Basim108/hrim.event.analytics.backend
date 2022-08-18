using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs;

public class GetOccurrenceEventTypeByIdHandler: IRequestHandler<GetOccurrenceEventTypeById, OccurrenceEventType?> {
    private readonly EventAnalyticDbContext _context;

    public GetOccurrenceEventTypeByIdHandler(EventAnalyticDbContext context) {
        _context = context;
    }
    public Task<OccurrenceEventType?> Handle(GetOccurrenceEventTypeById request, CancellationToken cancellation) {
        if (request.EventTypeId == default)
            throw new ArgumentNullException(nameof(request.EventTypeId));

        var query = _context.OccurrenceEventTypes.AsQueryable();
        if(request.IsNotTrackable) {
            query.AsNoTracking();
        }
        return query.FirstOrDefaultAsync(x => x.Id == request.EventTypeId,
                                         cancellation);
    }
}