using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class GetEventByIdHandler<TEvent>: IRequestHandler<GetEventById<TEvent>, CqrsResult<TEvent?>>
    where TEvent : BaseEvent, new() {
    private readonly EventAnalyticDbContext               _context;
    private readonly ILogger<GetEventByIdHandler<TEvent>> _logger;
    private readonly IMapper                              _mapper;

    public GetEventByIdHandler(EventAnalyticDbContext               context,
                               ILogger<GetEventByIdHandler<TEvent>> logger,
                               IMapper                              mapper) {
        _context = context;
        _logger  = logger;
        _mapper  = mapper;
    }

    public Task<CqrsResult<TEvent?>> Handle(GetEventById<TEvent> request, CancellationToken cancellationToken) {
        if (request.Id == Guid.Empty)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Id)}");
        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<TEvent?>> HandleAsync(GetEventById<TEvent> request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(CoreLogs.HRIM_ENTITY_ID, request.Id);
        try {
            BaseEvent? db;
            switch (new TEvent()) {
                case DurationEvent:
                    var durationQuery = _context.DurationEvents.AsQueryable();
                    if (request.IsNotTrackable) {
                        durationQuery = durationQuery.AsNoTracking();
                    }
                    db = await durationQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                    break;
                case OccurrenceEvent:
                    var occurrenceQuery = _context.OccurrenceEvents.AsQueryable();
                    if (request.IsNotTrackable) {
                        occurrenceQuery = occurrenceQuery.AsNoTracking();
                    }
                    db = await occurrenceQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                    break;
                default:
                    throw new UnsupportedEntityException(typeof(TEvent));
            }
            if (db == null) {
                return new CqrsResult<TEvent?>(null, CqrsResultCode.NotFound);
            }
            var result = _mapper.Map<TEvent>(db);
            if (result.IsDeleted == true) {
                return new CqrsResult<TEvent?>(result, CqrsResultCode.EntityIsDeleted);
            }
            if (result.CreatedById != request.Context.UserId) {
                return new CqrsResult<TEvent?>(result, CqrsResultCode.Forbidden);
            }
            return new CqrsResult<TEvent?>(result, CqrsResultCode.Ok);
        }
        catch (TimeoutException ex) {
            _logger.LogWarning(EfCoreLogs.OPERATION_TIMEOUT, HrimOperations.Read, ex.Message);
            return new CqrsResult<TEvent?>(null, CqrsResultCode.Locked);
        }
    }
}