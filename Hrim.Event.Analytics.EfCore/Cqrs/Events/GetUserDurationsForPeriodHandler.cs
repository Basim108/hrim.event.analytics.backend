using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Abstractions.ViewModels.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

public class GetUserDurationsForPeriodHandler: IRequestHandler<GetUserDurationsForPeriod, IList<ViewDurationEvent>> {
    private readonly EventAnalyticDbContext _context;

    public GetUserDurationsForPeriodHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public async Task<IList<ViewDurationEvent>> Handle(GetUserDurationsForPeriod request, CancellationToken cancellationToken) {
        var dbEntities = await _context.DurationEvents
                                       .Where(x => x.CreatedById == request.OwnerId &&
                                                   x.StartedOn   >= request.Start   &&
                                                   x.StartedOn   <= request.End     &&
                                                   x.IsDeleted   != true)
                                       .AsNoTracking()
                                       .Select(x => new {
                                            x.Id,
                                            x.StartedOn,
                                            x.StartedAt,
                                            x.FinishedOn,
                                            x.FinishedAt,
                                            x.EventTypeId
                                        })
                                       .ToListAsync(cancellationToken);
        var result = dbEntities.Select(x => new ViewDurationEvent(x.Id,
                                                                  x.StartedOn.CombineWithTime(x.StartedAt),
                                                                  x.FinishedOn?.CombineWithTime(x.FinishedAt!.Value),
                                                                  x.EventTypeId))
                               .ToList();
        return result;
    }
}