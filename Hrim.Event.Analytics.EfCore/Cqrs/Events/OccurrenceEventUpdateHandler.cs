using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class OccurrenceEventUpdateHandler: IRequestHandler<OccurrenceEventUpdateCommand, CqrsResult<OccurrenceEvent?>>
{
    private readonly ILogger<OccurrenceEventUpdateHandler> _logger;
    private readonly IMapper                               _mapper;
    private readonly EventAnalyticDbContext                _context;
    private readonly IApiRequestAccessor                   _requestAccessor;

    public OccurrenceEventUpdateHandler(ILogger<OccurrenceEventUpdateHandler> logger,
                                        IMapper                               mapper,
                                        EventAnalyticDbContext                context,
                                        IApiRequestAccessor                   requestAccessor) {
        _logger          = logger;
        _mapper          = mapper;
        _context         = context;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<OccurrenceEvent?>> Handle(OccurrenceEventUpdateCommand request, CancellationToken cancellationToken) {
        if (request.EventInfo == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventInfo)}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<OccurrenceEvent?>> HandleAsync(OccurrenceEventUpdateCommand request, CancellationToken cancellationToken) {
        var mappedEventInfo = _mapper.Map<DbOccurrenceEvent>(request.EventInfo);
        var existed = await _context.OccurrenceEvents
                                    .FirstOrDefaultAsync(x => x.Id == mappedEventInfo.Id,
                                                         cancellationToken);
        if (existed == null) {
            _logger.LogDebug(EfCoreLogs.ENTITY_NOT_FOUND_BY_ID, nameof(OccurrenceEvent));
            return new CqrsResult<OccurrenceEvent?>(null, CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted == true) {
            _logger.LogInformation(EfCoreLogs.CANNOT_UPDATE_ENTITY_IS_DELETED, existed.ConcurrentToken, nameof(OccurrenceEvent));
            var deletedEvent = _mapper.Map<OccurrenceEvent>(existed);
            return new CqrsResult<OccurrenceEvent?>(deletedEvent, CqrsResultCode.EntityIsDeleted);
        }
        if (existed.ConcurrentToken != request.EventInfo.ConcurrentToken) {
            _logger.LogInformation(EfCoreLogs.CONCURRENT_CONFLICT,
                                   HrimOperations.Update,
                                   existed.ConcurrentToken,
                                   request.EventInfo.ConcurrentToken,
                                   nameof(OccurrenceEvent));
            var conflictedEvent = _mapper.Map<OccurrenceEvent>(existed);
            return new CqrsResult<OccurrenceEvent?>(conflictedEvent, CqrsResultCode.Conflict);
        }
        var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellationToken);
        if (existed.CreatedById != operatorUserId) {
            _logger.LogWarning(EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Update, existed.CreatedById, nameof(OccurrenceEvent));
            var conflictedEvent = _mapper.Map<OccurrenceEvent>(existed);
            return new CqrsResult<OccurrenceEvent?>(conflictedEvent, CqrsResultCode.Forbidden);
        }
        var isChanged = false;
        if (existed.OccurredOn != mappedEventInfo.OccurredOn) {
            existed.OccurredOn = mappedEventInfo.OccurredOn;
            isChanged          = true;
        }
        if (!existed.OccurredAt.IsTimeEquals(mappedEventInfo.OccurredAt)) {
            existed.OccurredAt = mappedEventInfo.OccurredAt;
            isChanged          = true;
        }
        if (existed.EventTypeId != mappedEventInfo.EventTypeId) {
            existed.EventTypeId = request.EventInfo.EventTypeId;
            isChanged           = true;
        }
        if (isChanged) {
            existed.UpdatedAt = DateTime.UtcNow.TruncateToMicroseconds();
            existed.ConcurrentToken++;
            if (request.SaveChanges) {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        var updatedEvent = _mapper.Map<OccurrenceEvent>(existed);
        return new CqrsResult<OccurrenceEvent?>(updatedEvent, CqrsResultCode.Ok);
    }
}