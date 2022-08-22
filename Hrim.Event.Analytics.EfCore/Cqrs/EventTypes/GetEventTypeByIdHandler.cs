using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

public class GetEventTypeByIdHandler: IRequestHandler<GetEventTypeById, UserEventType?> {
    private readonly EventAnalyticDbContext _context;

    public GetEventTypeByIdHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public Task<UserEventType?> Handle(GetEventTypeById request, CancellationToken cancellationToken) {
        if (request.Id == default)
            throw new ArgumentNullException(nameof(request.Id));

        return HandleAsync(request, cancellationToken);
    }

    private async Task<UserEventType?> HandleAsync(GetEventTypeById request, CancellationToken cancellationToken) {
        var query = _context.UserEventTypes.AsQueryable();
        if (request.IsNotTrackable) {
            query = query.AsNoTracking();
        }
        var result = await query.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return result;
    }
}