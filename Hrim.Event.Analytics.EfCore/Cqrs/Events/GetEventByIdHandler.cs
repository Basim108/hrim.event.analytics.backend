using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class GetEventByIdHandler<TEvent>: IRequestHandler<GetEventById<TEvent>, CqrsResult<TEvent?>>
    where TEvent : BaseEvent, new()
{
    private readonly EventAnalyticDbContext               _context;
    private readonly ILogger<GetEventByIdHandler<TEvent>> _logger;
    private readonly IMapper                              _mapper;
    private readonly IApiRequestAccessor                  _requestAccessor;

    public GetEventByIdHandler(EventAnalyticDbContext               context,
                               ILogger<GetEventByIdHandler<TEvent>> logger,
                               IMapper                              mapper,
                               IApiRequestAccessor                  requestAccessor) {
        _context         = context;
        _logger          = logger;
        _mapper          = mapper;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<TEvent?>> Handle(GetEventById<TEvent> request, CancellationToken cancellationToken) {
        if (request.Id == default)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Id)}");
        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<CqrsResult<TEvent?>> HandleAsync(GetEventById<TEvent> request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(messageFormat: CoreLogs.HRIM_ENTITY_ID, request.Id);
        try {
            DbBaseEvent? db;
            switch (new TEvent()) {
                case DurationEvent:
                    var durationQuery = _context.DurationEvents.AsQueryable();
                    if (request.IsNotTrackable)
                        durationQuery = durationQuery.AsNoTracking();
                    db = await durationQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
                    break;
                case OccurrenceEvent:
                    var occurrenceQuery = _context.OccurrenceEvents.AsQueryable();
                    if (request.IsNotTrackable)
                        occurrenceQuery = occurrenceQuery.AsNoTracking();
                    db = await occurrenceQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
                    break;
                default:
                    throw new UnsupportedEntityException(typeof(TEvent));
            }
            if (db == null)
                return new CqrsResult<TEvent?>(Result: null, StatusCode: CqrsResultCode.NotFound);
            var result = _mapper.Map<TEvent>(source: db);
            if (result.IsDeleted == true)
                return new CqrsResult<TEvent?>(Result: result, StatusCode: CqrsResultCode.EntityIsDeleted);
            var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
            if (result.CreatedById != operatorUserId) {
                _logger.LogWarning(message: EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Read, result.CreatedById, typeof(TEvent).Name);
                return new CqrsResult<TEvent?>(Result: result, StatusCode: CqrsResultCode.Forbidden);
            }
            return new CqrsResult<TEvent?>(Result: result, StatusCode: CqrsResultCode.Ok);
        }
        catch (TimeoutException ex) {
            _logger.LogWarning(message: EfCoreLogs.OPERATION_TIMEOUT, HrimOperations.Read, ex.Message);
            return new CqrsResult<TEvent?>(Result: null, StatusCode: CqrsResultCode.Locked);
        }
    }
}