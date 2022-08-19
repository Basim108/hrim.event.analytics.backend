using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

public class UpdateEventTypeHandler: IRequestHandler<UpdateEventTypeCommand, CqrsResult<SystemEventType?>> {
    private readonly ILogger<UpdateEventTypeHandler> _logger;
    private readonly IMapper                         _mapper;
    private readonly EventAnalyticDbContext          _context;

    public UpdateEventTypeHandler(ILogger<UpdateEventTypeHandler> logger,
                                  IMapper                         mapper,
                                  EventAnalyticDbContext          context) {
        _logger  = logger;
        _mapper  = mapper;
        _context = context;
    }

    public async Task<CqrsResult<SystemEventType?>> Handle(UpdateEventTypeCommand request, CancellationToken cancellation) {
        if (request.EventType == null)
            throw new ArgumentNullException(nameof(request.EventType));

        SystemEventType? existed = request.EventType switch {
            DurationEventType => await _context.DurationEventTypes
                                               .FirstOrDefaultAsync(x => x.Id == request.EventType.Id,
                                                                    cancellation),
            OccurrenceEventType => await _context.OccurrenceEventTypes
                                                 .FirstOrDefaultAsync(x => x.Id == request.EventType.Id,
                                                                      cancellation),
            _ => throw new UnsupportedEntityException(request.EventType.GetType())
        };
        if (existed == null) {
            _logger.LogDebug(EfCoreLogs.EntityNotFoundById, request.EventType.Id, nameof(OccurrenceEventType));
            return new CqrsResult<SystemEventType?>(null, CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted == true) {
            _logger.LogInformation(EfCoreLogs.CannotUpdateEntityIsDeleted, request.EventType.Id, existed.ConcurrentToken, existed.GetType().Name);
            return new CqrsResult<SystemEventType?>(existed, CqrsResultCode.EntityIsDeleted);
        }
        if (existed.ConcurrentToken != request.EventType.ConcurrentToken) {
            _logger.LogInformation(EfCoreLogs.CannotUpdateEntityIsDeleted, request.EventType.Id, existed.ConcurrentToken, existed.GetType().Name);
            return new CqrsResult<SystemEventType?>(existed, CqrsResultCode.Conflict);
        }
        existed.ConcurrentToken++;
        existed.Color    = request.EventType.Color;
        existed.Name     = request.EventType.Name;
        existed.IsPublic = request.EventType.IsPublic;
        existed.Description = string.IsNullOrWhiteSpace(request.EventType.Description)
                                  ? null
                                  : request.EventType.Description;
        existed.UpdatedAt = DateTime.UtcNow.TruncateToMicroseconds();
        SystemEventType? result;
        switch (request.EventType) {
            case DurationEventType duration: {
                var dbDuration      = _mapper.Map<DbDurationEventType>(duration);
                var existedDuration = existed as DbDurationEventType;
                existedDuration!.StartedOn = dbDuration.StartedOn;
                existedDuration.StartedAt  = dbDuration.StartedAt;
                existedDuration.FinishedOn = dbDuration.FinishedOn;
                existedDuration.FinishedAt = dbDuration.FinishedAt;

                result = _mapper.Map<DurationEventType>(existedDuration);
                break;
            }
            case OccurrenceEventType occurrence:
                var dbOccurrence      = _mapper.Map<DbOccurrenceEventType>(occurrence);
                var existedOccurrence = existed as DbOccurrenceEventType;
                existedOccurrence!.OccurredOn = dbOccurrence.OccurredOn;
                existedOccurrence.OccurredAt  = dbOccurrence.OccurredAt;

                result = _mapper.Map<OccurrenceEventType>(existedOccurrence);
                break;
            default:
                throw new UnsupportedEntityException(request.EventType.GetType());
        }
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellation);
        return new CqrsResult<SystemEventType?>(result, CqrsResultCode.Ok);
    }
}