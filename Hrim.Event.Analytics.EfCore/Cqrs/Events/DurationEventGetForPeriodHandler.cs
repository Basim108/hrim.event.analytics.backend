using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Extensions;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

public class DurationEventGetForPeriodHandler: IRequestHandler<DurationEventGetForPeriod, IList<ViewDurationEvent>>
{
    private readonly EventAnalyticDbContext _context;
    private readonly IApiRequestAccessor    _requestAccessor;

    public DurationEventGetForPeriodHandler(EventAnalyticDbContext context,
                                            IApiRequestAccessor    requestAccessor) {
        _context         = context;
        _requestAccessor = requestAccessor;
    }

    public async Task<IList<ViewDurationEvent>> Handle(DurationEventGetForPeriod request, CancellationToken cancellationToken) {
        var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
        var dbEntities = await _context.DurationEvents
                                       .Include(x => x.EventType)
                                       .Where(x => x.CreatedById == operatorUserId
                                                && (x.StartedOn <= request.Start && x.FinishedOn > request.Start
                                                 || x.StartedOn >= request.Start && x.StartedOn  <= request.End
                                                 || x.StartedOn < request.End    && x.FinishedOn >= request.End)
                                                && x.IsDeleted != true)
                                       .AsNoTracking()
                                       .ToListAsync(cancellationToken: cancellationToken);
        var result = dbEntities.Select(x => new ViewDurationEvent(Id: x.Id,
                                                                  x.StartedOn.CombineWithTime(time: x.StartedAt),
                                                                  x.FinishedOn?.CombineWithTime(time: x.FinishedAt!.Value),
                                                                  new ViewEventType(Id: x.EventType!.Id,
                                                                                    ParentId: x.EventType!.ParentId,
                                                                                    Name: x.EventType.Name,
                                                                                    Description: x.EventType.Description,
                                                                                    Color: x.EventType.Color,
                                                                                    IsPublic: x.EventType.IsPublic,
                                                                                    x.EventType.IsDeleted ?? false,
                                                                                    IsMine: true),
                                                                  ConcurrentToken: x.ConcurrentToken,
                                                                  Props: x.Props))
                               .ToList();
        return result;
    }
}