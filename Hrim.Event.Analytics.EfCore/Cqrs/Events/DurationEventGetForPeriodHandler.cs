using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

public class DurationEventGetForPeriodHandler: IRequestHandler<DurationEventGetForPeriod, IList<ViewDurationEvent>>
{
    private readonly EventAnalyticDbContext _context;

    public DurationEventGetForPeriodHandler(EventAnalyticDbContext context) { _context = context; }

    public async Task<IList<ViewDurationEvent>> Handle(DurationEventGetForPeriod request, CancellationToken cancellationToken) {
        var dbEntities = await _context.DurationEvents
                                       .Include(x => x.EventType)
                                       .Where(x => x.CreatedById == request.Context.UserId
                                                && (x.StartedOn <= request.Start && x.FinishedOn > request.Start
                                                 || x.StartedOn >= request.Start && x.StartedOn  <= request.End
                                                 || x.StartedOn < request.End    && x.FinishedOn >= request.End)
                                                && x.IsDeleted != true)
                                       .AsNoTracking()
                                       .ToListAsync(cancellationToken);
        var result = dbEntities.Select(x
                                           => new ViewDurationEvent(x.Id,
                                                                    x.StartedOn.CombineWithTime(x.StartedAt),
                                                                    x.FinishedOn?.CombineWithTime(x.FinishedAt!.Value),
                                                                    new ViewEventType(x.EventType!.Id,
                                                                                      x.EventType.Name,
                                                                                      x.EventType.Description,
                                                                                      x.EventType.Color,
                                                                                      x.EventType.IsPublic,
                                                                                      x.EventType.IsDeleted ?? false,
                                                                                      true),
                                                                    x.ConcurrentToken))
                               .ToList();
        return result;
    }
}