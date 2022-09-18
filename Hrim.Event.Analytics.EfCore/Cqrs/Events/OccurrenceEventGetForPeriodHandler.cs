using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

public class OccurrenceEventGetForPeriodHandler: IRequestHandler<OccurrenceEventGetForPeriod, IList<ViewOccurrenceEvent>> {
    private readonly EventAnalyticDbContext _context;

    public OccurrenceEventGetForPeriodHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public async Task<IList<ViewOccurrenceEvent>> Handle(OccurrenceEventGetForPeriod request, CancellationToken cancellationToken) {
        var dbEntities = await _context.OccurrenceEvents
                                       .Include(x => x.EventType)
                                       .Where(x => x.CreatedById == request.Context.UserId &&
                                                   x.OccurredOn  >= request.Start          &&
                                                   x.OccurredOn  <= request.End            &&
                                                   x.IsDeleted   != true)
                                       .AsNoTracking()
                                       .Select(x => new {
                                            x.Id,
                                            x.OccurredOn,
                                            x.OccurredAt,
                                            x.EventTypeId,
                                            x.EventType!.Color,
                                            x.ConcurrentToken
                                        })
                                       .ToListAsync(cancellationToken);
        var result = dbEntities.Select(x => new ViewOccurrenceEvent(x.Id,
                                                                    x.OccurredOn.CombineWithTime(x.OccurredAt),
                                                                    x.EventTypeId,
                                                                    x.Color,
                                                                    x.ConcurrentToken))
                               .ToList();
        return result;
    }
}