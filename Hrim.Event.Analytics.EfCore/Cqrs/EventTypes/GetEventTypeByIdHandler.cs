using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

public class GetEventTypeByIdHandler: IRequestHandler<GetEventTypeById, CqrsResult<UserEventType?>> {
    private readonly EventAnalyticDbContext           _context;
    private readonly ILogger<GetEventTypeByIdHandler> _logger;

    public GetEventTypeByIdHandler(EventAnalyticDbContext           context,
                                   ILogger<GetEventTypeByIdHandler> logger) {
        _context = context;
        _logger  = logger;
    }

    public Task<CqrsResult<UserEventType?>> Handle(GetEventTypeById request, CancellationToken cancellationToken) {
        if (request.Id == default)
            throw new ArgumentNullException(nameof(request.Id));

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<UserEventType?>> HandleAsync(GetEventTypeById request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(CoreLogs.HrimEntityId, request.Id);
        var       query         = _context.UserEventTypes.AsQueryable();
        if (request.IsNotTrackable) {
            query = query.AsNoTracking();
        }
        try {
            var result = await query.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (result == null) {
                return new CqrsResult<UserEventType?>(null, CqrsResultCode.NotFound);
            }
            if (result.IsDeleted == true) {
                return new CqrsResult<UserEventType?>(result, CqrsResultCode.EntityIsDeleted);
            }
            if (result.CreatedById != request.Context.UserId) {
                return new CqrsResult<UserEventType?>(result, CqrsResultCode.Forbidden);
            }
            return new CqrsResult<UserEventType?>(result, CqrsResultCode.Ok);
        }
        catch (TimeoutException ex) {
            _logger.LogWarning(ex.ToString());
            return new CqrsResult<UserEventType?>(null, CqrsResultCode.Locked);
        }
    }
}