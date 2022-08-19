using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs;

public class GetViewDurationEventTypesHandler: IRequestHandler<GetViewDurationEventTypes, IList<ViewSystemEventType>> {
    private readonly EventAnalyticDbContext _context;

    public GetViewDurationEventTypesHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public async Task<IList<ViewSystemEventType>> Handle(GetViewDurationEventTypes request, CancellationToken cancellation) {
        var query = _context.DurationEventTypes.AsQueryable();
        if(request.CreatedById.HasValue)
            query = query.Where(x => x.CreatedById == request.CreatedById);
        if(request.IsPublic)
            query = query.Where(x => x.IsPublic == true);
        if(request.IsDeleted)
            query = query.Where(x => x.IsDeleted == true);

        var durationTypes = await query.AsNoTracking()
                                       .Select(x => new {
                                            x.Id,
                                            x.StartedOn,
                                            x.StartedAt,
                                            x.FinishedOn,
                                            x.FinishedAt,
                                            x.Name,
                                            x.Description,
                                            x.Color,
                                            x.IsPublic
                                        })
                                       .ToListAsync(cancellation);
        var durationViews = durationTypes.Select(x => new ViewDurationEventType(x.Id,
                                                                                x.StartedOn.CombineWithTime(x.StartedAt),
                                                                                x.FinishedOn.CombineWithTime(x.FinishedAt),
                                                                                x.Name,
                                                                                x.Description,
                                                                                x.Color,
                                                                                x.IsPublic))
                                         .Cast<ViewSystemEventType>()
                                         .ToList();
        return durationViews;
    }
}