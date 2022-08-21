using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

public class GetViewEventTypesHandler: IRequestHandler<GetViewEventTypes, IList<ViewSystemEventType>> {
    private readonly EventAnalyticDbContext _context;

    public GetViewEventTypesHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public async Task<IList<ViewSystemEventType>> Handle(GetViewEventTypes request, CancellationToken cancellationToken) {
        var query = _context.UserEventTypes.AsQueryable();
        if (request.CreatedById != default) {
            query = query.Where(x => x.CreatedById == request.CreatedById);
        }
        if (request.IsPublic) {
            query = query.Where(x => x.IsPublic == true);
        }
        if (!request.IncludeDeleted) {
            query = query.Where(x => x.IsDeleted != true);
        }
        var result = await query.AsNoTracking()
                                .Select(x => new ViewSystemEventType(x.Id,
                                                                     x.Name,
                                                                     x.Description,
                                                                     x.Color,
                                                                     x.IsPublic,
                                                                     x.IsDeleted == true))
                                .ToListAsync(cancellationToken);
        return result;
    }
}