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

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class DurationEventUpdateHandler: IRequestHandler<DurationEventUpdateCommand, CqrsResult<DurationEvent?>>
{
    private readonly EventAnalyticDbContext              _context;
    private readonly ILogger<DurationEventUpdateHandler> _logger;
    private readonly IMapper                             _mapper;
    private readonly IApiRequestAccessor                 _requestAccessor;

    public DurationEventUpdateHandler(ILogger<DurationEventUpdateHandler> logger,
                                      IMapper                             mapper,
                                      IApiRequestAccessor                 requestAccessor,
                                      EventAnalyticDbContext              context) {
        _logger          = logger;
        _mapper          = mapper;
        _requestAccessor = requestAccessor;
        _context         = context;
    }

    public Task<CqrsResult<DurationEvent?>> Handle(DurationEventUpdateCommand request, CancellationToken cancellationToken) {
        if (request.EventInfo == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventInfo)}");

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<CqrsResult<DurationEvent?>> HandleAsync(DurationEventUpdateCommand request, CancellationToken cancellationToken) {
        var mappedEventInfo = _mapper.Map<DbDurationEvent>(source: request.EventInfo);
        var existed = await _context.DurationEvents
                                    .FirstOrDefaultAsync(x => x.Id == mappedEventInfo.Id,
                                                         cancellationToken: cancellationToken);
        if (existed == null) {
            _logger.LogDebug(message: EfCoreLogs.ENTITY_NOT_FOUND_BY_ID, nameof(DurationEvent));
            return new CqrsResult<DurationEvent?>(Result: null, StatusCode: CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted == true) {
            _logger.LogInformation(message: EfCoreLogs.CANNOT_UPDATE_ENTITY_IS_DELETED, existed.ConcurrentToken, nameof(DurationEvent));
            var deletedEvent = _mapper.Map<DurationEvent>(source: existed);
            return new CqrsResult<DurationEvent?>(Result: deletedEvent, StatusCode: CqrsResultCode.EntityIsDeleted);
        }
        if (existed.ConcurrentToken != request.EventInfo.ConcurrentToken) {
            _logger.LogInformation(message: EfCoreLogs.CONCURRENT_CONFLICT,
                                   HrimOperations.Update,
                                   existed.ConcurrentToken,
                                   request.EventInfo.ConcurrentToken,
                                   nameof(DurationEvent));
            var conflictedEvent = _mapper.Map<DurationEvent>(source: existed);
            return new CqrsResult<DurationEvent?>(Result: conflictedEvent, StatusCode: CqrsResultCode.Conflict);
        }
        var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
        if (existed.CreatedById != operatorUserId) {
            _logger.LogWarning(message: EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Update, existed.CreatedById, nameof(DurationEvent));
            var conflictedEvent = _mapper.Map<DurationEvent>(source: existed);
            return new CqrsResult<DurationEvent?>(Result: conflictedEvent, StatusCode: CqrsResultCode.Forbidden);
        }
        var isChanged = false;
        if (existed.StartedOn != mappedEventInfo.StartedOn) {
            existed.StartedOn = mappedEventInfo.StartedOn;
            isChanged         = true;
        }
        if (!existed.StartedAt.IsTimeEquals(another: mappedEventInfo.StartedAt)) {
            existed.StartedAt = mappedEventInfo.StartedAt;
            isChanged         = true;
        }
        if (existed.FinishedOn != mappedEventInfo.FinishedOn) {
            existed.FinishedOn = mappedEventInfo.FinishedOn;
            isChanged          = true;
        }
        if (!existed.FinishedAt.IsTimeEquals(another: mappedEventInfo.FinishedAt)) {
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
            if (request.SaveChanges)
                await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        }
        var updatedEvent = _mapper.Map<DurationEvent>(source: existed);
        return new CqrsResult<DurationEvent?>(Result: updatedEvent, StatusCode: CqrsResultCode.Ok);
    }
}