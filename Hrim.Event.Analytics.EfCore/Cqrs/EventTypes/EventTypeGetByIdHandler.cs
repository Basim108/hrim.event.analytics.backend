using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EventType = Hrim.Event.Analytics.Abstractions.Entities.EventTypes.EventType;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class EventTypeGetByIdHandler: IRequestHandler<EventTypeGetById, CqrsResult<EventType?>>
{
    private readonly EventAnalyticDbContext           _context;
    private readonly ILogger<EventTypeGetByIdHandler> _logger;
    private readonly IMapper                          _mapper;
    private readonly IApiRequestAccessor              _requestAccessor;

    public EventTypeGetByIdHandler(EventAnalyticDbContext           context,
                                   ILogger<EventTypeGetByIdHandler> logger,
                                   IMapper                          mapper,
                                   IApiRequestAccessor              requestAccessor) {
        _context         = context;
        _logger          = logger;
        _mapper          = mapper;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<EventType?>> Handle(EventTypeGetById request, CancellationToken cancellationToken) {
        if (request.Id == default)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Id)}");

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<CqrsResult<EventType?>> HandleAsync(EventTypeGetById request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(messageFormat: CoreLogs.HRIM_ENTITY_ID, request.Id);
        var       query         = _context.EventTypes.AsQueryable();
        if (request.IsNotTrackable)
            query = query.AsNoTracking();
        try {
            var dbEntityResult = await query.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
            if (dbEntityResult == null)
                return new CqrsResult<EventType?>(Result: null, StatusCode: CqrsResultCode.NotFound);
            var blEntityResult = _mapper.Map<EventType>(dbEntityResult);
            if (dbEntityResult.IsDeleted == true)
                return new CqrsResult<EventType?>(Result: blEntityResult, StatusCode: CqrsResultCode.EntityIsDeleted);
            var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
            if (dbEntityResult.CreatedById != operatorUserId) {
                _logger.LogWarning(message: EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Read, dbEntityResult.CreatedById, nameof(EventType));
                return new CqrsResult<EventType?>(Result: blEntityResult, StatusCode: CqrsResultCode.Forbidden);
            }
            return new CqrsResult<EventType?>(Result: blEntityResult, StatusCode: CqrsResultCode.Ok);
        }
        catch (TimeoutException ex) {
            _logger.LogWarning(message: EfCoreLogs.OPERATION_TIMEOUT, HrimOperations.Read, ex.Message);
            return new CqrsResult<EventType?>(Result: null, StatusCode: CqrsResultCode.Locked);
        }
    }
}