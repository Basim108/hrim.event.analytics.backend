using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class EventTypesGetAllMineHandler: IRequestHandler<EventTypeGetAllMine, IList<ViewEventType>>
{
    private readonly EventAnalyticDbContext _context;
    private readonly IApiRequestAccessor    _requestAccessor;

    public EventTypesGetAllMineHandler(EventAnalyticDbContext context,
                                       IApiRequestAccessor    requestAccessor) {
        _context         = context;
        _requestAccessor = requestAccessor;
    }

    public Task<IList<ViewEventType>> Handle(EventTypeGetAllMine request, CancellationToken cancellationToken) {
        if (request.Context == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}");

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<IList<ViewEventType>> HandleAsync(EventTypeGetAllMine request, CancellationToken cancellationToken) {
        var query = _context.UserEventTypes.AsQueryable();

        var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
        if (request.IncludeOthersPublic)
            query = query.Where(x => x.IsPublic || !x.IsPublic && x.CreatedById == operatorUserId);
        else
            query = query.Where(x => x.CreatedById == operatorUserId);
        if (!request.IncludeDeleted)
            query = query.Where(x => x.IsDeleted != true);
        var result = await query.AsNoTracking()
                                .Select(x => new ViewEventType(x.Id,
                                                               x.Name,
                                                               x.Description,
                                                               x.Color,
                                                               x.IsPublic,
                                                               x.IsDeleted   == true,
                                                               x.CreatedById == operatorUserId))
                                .ToListAsync(cancellationToken: cancellationToken);
        return result;
    }
}