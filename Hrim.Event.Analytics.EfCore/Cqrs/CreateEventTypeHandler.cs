using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs;

public class CreateEventTypeHandler: IRequestHandler<CreateEventTypeCommand, CqrsResult<SystemEventType?>> {
    private readonly ILogger<CreateEventTypeHandler> _logger;
    private readonly IMapper                         _mapper;
    private readonly EventAnalyticDbContext          _context;

    public CreateEventTypeHandler(ILogger<CreateEventTypeHandler> logger,
                                  IMapper                         mapper,
                                  EventAnalyticDbContext          context) {
        _logger  = logger;
        _mapper  = mapper;
        _context = context;
    }

    public async Task<CqrsResult<SystemEventType?>> Handle(CreateEventTypeCommand request, CancellationToken cancellation) {
        if (request.EventType == null)
            throw new ArgumentNullException(nameof(request.EventType));

        SystemEventType? existed = request.EventType switch {
            DurationEventType => await _context.DurationEventTypes
                                               .FirstOrDefaultAsync(x => x.CreatedById == request.EventType.CreatedById &&
                                                                         x.Name        == request.EventType.Name,
                                                                    cancellation),
            OccurrenceEventType =>
                await _context.OccurrenceEventTypes
                              .FirstOrDefaultAsync(x => x.CreatedById == request.EventType.CreatedById &&
                                                        x.Name        == request.EventType.Name,
                                                   cancellation),
            _ => throw new UnsupportedEntityException(request.EventType.GetType())
        };
        if (existed != null) {
            if (existed.IsDeleted == true) {
                _logger.LogDebug(EfCoreLogs.CannotCreateIsDeleted, existed.GetType().Name);
                return new CqrsResult<SystemEventType?>(existed, CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogDebug(EfCoreLogs.CannotCreateIsAlreadyExisted + existed, existed.GetType().Name);
            return new CqrsResult<SystemEventType?>(null, CqrsResultCode.Conflict);
        }
        request.EventType.CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds();
        request.EventType.UpdatedAt       = null;
        request.EventType.ConcurrentToken = 1;
        request.EventType.Description = string.IsNullOrWhiteSpace(request.EventType.Description)
                                            ? null
                                            : request.EventType.Description;
        Entity db;
        switch (request.EventType) {
            case DurationEventType duration:
                var dbDuration = _mapper.Map<DbDurationEventType>(duration);
                _context.DurationEventTypes.Add(dbDuration);
                db = dbDuration;
                break;
            case OccurrenceEventType occurrence:
                var dbOccurrence = _mapper.Map<DbOccurrenceEventType>(occurrence);
                _context.OccurrenceEventTypes.Add(dbOccurrence);
                db = dbOccurrence;
                break;
            default:
                throw new UnsupportedEntityException(request.EventType.GetType());
        }
        if (request.SaveChanges) {
            await _context.SaveChangesAsync(cancellation);
            request.EventType.Id = db.Id;
        }

        return new CqrsResult<SystemEventType?>(request.EventType, CqrsResultCode.Created);
    }
}