using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class GetViewEventTypesHandler: IRequestHandler<GetViewEventTypes, IList<ViewEventType>> {
    private readonly EventAnalyticDbContext _context;

    public GetViewEventTypesHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public async Task<IList<ViewEventType>> Handle(GetViewEventTypes request, CancellationToken cancellationToken) {
        var query = _context.UserEventTypes.AsQueryable();
        if (request.CreatedById != Guid.Empty) {
            query = query.Where(x => x.CreatedById == request.CreatedById);
        }
        if (request.IsPublic) {
            query = query.Where(x => x.IsPublic);
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
                                                                     x.IsDeleted == true))
                                .ToListAsync(cancellationToken);
        return result;
    }
}