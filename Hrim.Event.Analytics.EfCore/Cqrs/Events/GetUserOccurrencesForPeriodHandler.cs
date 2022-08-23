using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Abstractions.ViewModels.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

public class GetUserOccurrencesForPeriodHandler: IRequestHandler<GetUserOccurrencesForPeriod, IList<ViewOccurrenceEvent>> {
    private readonly EventAnalyticDbContext _context;

    public GetUserOccurrencesForPeriodHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public async Task<IList<ViewOccurrenceEvent>> Handle(GetUserOccurrencesForPeriod request, CancellationToken cancellationToken) {
        var dbEntities = await _context.OccurrenceEvents
                                       .Where(x => x.CreatedById == request.Context.UserId &&
                                                   x.OccurredOn  >= request.Start          &&
                                                   x.OccurredOn  <= request.End            &&
                                                   x.IsDeleted   != true)
                                       .AsNoTracking()
                                       .Select(x => new {
                                            x.Id,
                                            x.OccurredOn,
                                            x.OccurredAt,
                                            x.EventTypeId
                                        })
                                       .ToListAsync(cancellationToken);
        var result = dbEntities.Select(x => new ViewOccurrenceEvent(x.Id,
                                                                    x.OccurredOn.CombineWithTime(x.OccurredAt),
                                                                    x.EventTypeId))
                               .ToList();
        return result;
    }
}