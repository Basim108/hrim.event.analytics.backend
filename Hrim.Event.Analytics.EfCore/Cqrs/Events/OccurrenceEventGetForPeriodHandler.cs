using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

public class OccurrenceEventGetForPeriodHandler: IRequestHandler<OccurrenceEventGetForPeriod, IList<ViewOccurrenceEvent>>
{
    private readonly EventAnalyticDbContext _context;
    private readonly IApiRequestAccessor    _requestAccessor;

    public OccurrenceEventGetForPeriodHandler(EventAnalyticDbContext context,
                                              IApiRequestAccessor    requestAccessor) {
        _context         = context;
        _requestAccessor = requestAccessor;
    }

    public async Task<IList<ViewOccurrenceEvent>> Handle(OccurrenceEventGetForPeriod request, CancellationToken cancellationToken) {
        var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellationToken);
        var dbEntities = await _context.OccurrenceEvents
                                       .Include(x => x.EventType)
                                       .Where(x => x.CreatedById == operatorUserId
                                                && x.OccurredOn  >= request.Start
                                                && x.OccurredOn  <= request.End
                                                && x.IsDeleted   != true)
                                       .AsNoTracking()
                                       .ToListAsync(cancellationToken);
        var result = dbEntities.Select(x => new ViewOccurrenceEvent(x.Id,
                                                                    x.OccurredOn.CombineWithTime(x.OccurredAt),
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