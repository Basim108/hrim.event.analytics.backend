using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class DurationEventUpdateHandler: IRequestHandler<DurationEventUpdateCommand, CqrsResult<DurationEvent?>> {
    private readonly ILogger<DurationEventUpdateHandler> _logger;
    private readonly IMapper                             _mapper;
    private readonly EventAnalyticDbContext              _context;

    public DurationEventUpdateHandler(ILogger<DurationEventUpdateHandler> logger,
                                      IMapper                             mapper,
                                      EventAnalyticDbContext              context) {
        _logger  = logger;
        _mapper  = mapper;
        _context = context;
    }

    public Task<CqrsResult<DurationEvent?>> Handle(DurationEventUpdateCommand request, CancellationToken cancellationToken) {
        if (request.EventInfo == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventInfo)}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<DurationEvent?>> HandleAsync(DurationEventUpdateCommand request, CancellationToken cancellationToken) {
        var mappedEventInfo = _mapper.Map<DbDurationEvent>(request.EventInfo);
        var existed = await _context.DurationEvents
                                    .FirstOrDefaultAsync(x => x.Id == mappedEventInfo.Id,
                                                         cancellationToken);
        if (existed == null) {
            _logger.LogDebug(EfCoreLogs.ENTITY_NOT_FOUND_BY_ID, nameof(DurationEvent));
            return new CqrsResult<DurationEvent?>(null, CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted == true) {
            _logger.LogInformation(EfCoreLogs.CANNOT_UPDATE_ENTITY_IS_DELETED, existed.ConcurrentToken, nameof(DurationEvent));
            var deletedEvent = _mapper.Map<DurationEvent>(existed);
            return new CqrsResult<DurationEvent?>(deletedEvent, CqrsResultCode.EntityIsDeleted);
        }
        if (existed.ConcurrentToken != request.EventInfo.ConcurrentToken) {
            _logger.LogInformation(EfCoreLogs.CONCURRENT_CONFLICT, 
                                   HrimOperations.Update, 
                                   existed.ConcurrentToken, 
                                   request.EventInfo.ConcurrentToken, 
                                   nameof(DurationEvent));
            var conflictedEvent = _mapper.Map<DurationEvent>(existed);
            return new CqrsResult<DurationEvent?>(conflictedEvent, CqrsResultCode.Conflict);
        }
        if(existed.CreatedById != request.Context.UserId) {
            _logger.LogWarning(EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Update, existed.CreatedById, nameof(DurationEvent));
            var conflictedEvent = _mapper.Map<DurationEvent>(existed);
            return new CqrsResult<DurationEvent?>(conflictedEvent, CqrsResultCode.Forbidden);
        }
        var isChanged = false;
        if (existed.StartedOn != mappedEventInfo.StartedOn) {
            existed.StartedOn = mappedEventInfo.StartedOn;
            isChanged         = true;
        }
        if (!existed.StartedAt.IsTimeEquals(mappedEventInfo.StartedAt)) {
            existed.StartedAt = mappedEventInfo.StartedAt;
            isChanged         = true;
        }
        if (existed.FinishedOn != mappedEventInfo.FinishedOn) {
            existed.FinishedOn = mappedEventInfo.FinishedOn;
            isChanged          = true;
        }
        if (!existed.FinishedAt.IsTimeEquals(mappedEventInfo.FinishedAt)) {
            existed.FinishedAt = mappedEventInfo.FinishedAt;
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
        var updatedEvent = _mapper.Map<DurationEvent>(existed);
        return new CqrsResult<DurationEvent?>(updatedEvent, CqrsResultCode.Ok);
    }
}