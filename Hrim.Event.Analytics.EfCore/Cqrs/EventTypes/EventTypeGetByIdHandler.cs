using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class EventTypeGetByIdHandler: IRequestHandler<EventTypeGetById, CqrsResult<UserEventType?>>
{
    private readonly EventAnalyticDbContext           _context;
    private readonly ILogger<EventTypeGetByIdHandler> _logger;
    private readonly IApiRequestAccessor              _requestAccessor;

    public EventTypeGetByIdHandler(EventAnalyticDbContext           context,
                                   ILogger<EventTypeGetByIdHandler> logger,
                                   IApiRequestAccessor              requestAccessor) {
        _context         = context;
        _logger          = logger;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<UserEventType?>> Handle(EventTypeGetById request, CancellationToken cancellationToken) {
        if (request.Id == Guid.Empty)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Id)}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<UserEventType?>> HandleAsync(EventTypeGetById request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(CoreLogs.HRIM_ENTITY_ID, request.Id);
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
            var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellationToken);
            if (result.CreatedById != operatorUserId) {
                _logger.LogWarning(EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Read, result.CreatedById, nameof(UserEventType));
                return new CqrsResult<UserEventType?>(result, CqrsResultCode.Forbidden);
            }
            return new CqrsResult<UserEventType?>(result, CqrsResultCode.Ok);
        }
        catch (TimeoutException ex) {
            _logger.LogWarning(EfCoreLogs.OPERATION_TIMEOUT, HrimOperations.Read, ex.Message);
            return new CqrsResult<UserEventType?>(null, CqrsResultCode.Locked);
        }
    }
}