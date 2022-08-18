using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs;

public class GetDurationEventTypeByIdHandler: IRequestHandler<GetDurationEventTypeById, DurationEventType?> {
    private readonly EventAnalyticDbContext _context;

    public GetDurationEventTypeByIdHandler(EventAnalyticDbContext context) {
        _context = context;
    }
    public Task<DurationEventType?> Handle(GetDurationEventTypeById request, CancellationToken cancellation) {
        if (request.EventTypeId == default)
            throw new ArgumentNullException(nameof(request.EventTypeId));

        var query = _context.DurationEventTypes.AsQueryable();
        if(request.IsNotTrackable) {
            query.AsNoTracking();
        }
        return query.FirstOrDefaultAsync(x => x.Id == request.EventTypeId,
                                         cancellation);
    }
}