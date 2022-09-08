using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class EventTypesGetAllMineHandler: IRequestHandler<EventTypeGetAllMine, IList<ViewEventType>> {
    private readonly EventAnalyticDbContext _context;

    public EventTypesGetAllMineHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public Task<IList<ViewEventType>> Handle(EventTypeGetAllMine request, CancellationToken cancellationToken) {
        if (request.Context == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}");
        if (request.Context.UserId == Guid.Empty)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}.{nameof(request.Context.UserId)}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<IList<ViewEventType>> HandleAsync(EventTypeGetAllMine request, CancellationToken cancellationToken) {
        var query = _context.UserEventTypes.AsQueryable();

        if (request.IncludeOthersPublic) {
            query = query.Where(x => x.IsPublic ||
                                     (!x.IsPublic && x.CreatedById == request.Context.UserId));
        }
        else {
            query = query.Where(x => x.CreatedById == request.Context.UserId);
        }
        if (!request.IncludeDeleted) {
            query = query.Where(x => x.IsDeleted != true);
        }
        var result = await query.AsNoTracking()
                                .Select(x => new ViewEventType(x.Id,
                                                               x.Name,
                                                               x.Description,
                                                               x.Color,
                                                               x.IsPublic,
                                                               x.IsDeleted == true,
                                                               x.CreatedById == request.Context.UserId))
                                .ToListAsync(cancellationToken);
        return result;
    }
}