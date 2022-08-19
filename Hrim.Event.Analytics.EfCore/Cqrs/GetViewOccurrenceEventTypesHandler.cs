using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs;

public class GetViewOccurrenceEventTypesHandler: IRequestHandler<GetViewOccurrenceEventTypes, IList<ViewSystemEventType>> {
    private readonly EventAnalyticDbContext _context;

    public GetViewOccurrenceEventTypesHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public async Task<IList<ViewSystemEventType>> Handle(GetViewOccurrenceEventTypes request, CancellationToken cancellation) {
        var query = _context.OccurrenceEventTypes.AsQueryable();
        if (request.CreatedById.HasValue)
            query = query.Where(x => x.CreatedById == request.CreatedById);
        if (request.IsPublic)
            query = query.Where(x => x.IsPublic == true);
        if (request.IsDeleted)
            query = query.Where(x => x.IsDeleted == true);

        var durationTypes = await query.AsNoTracking()
                                       .Select(x => new {
                                                   x.Id,
                                                   x.OccurredOn,
                                                   x.OccurredAt,
                                                   x.Name,
                                                   x.Description,
                                                   x.Color,
                                                   x.IsPublic
                                               })
                                       .ToListAsync(cancellation);
        var durationViews = durationTypes.Select(x => new ViewOccurrenceEventType(x.Id,
                                                                                  x.OccurredOn.CombineWithTime(x.OccurredAt),
                                                                                  x.Name,
                                                                                  x.Description,
                                                                                  x.Color,
                                                                                  x.IsPublic))
                                         .Cast<ViewSystemEventType>()
                                         .ToList();
        return durationViews;
    }
}