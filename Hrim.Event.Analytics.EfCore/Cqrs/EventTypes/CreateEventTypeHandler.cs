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

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

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

    public Task<CqrsResult<SystemEventType?>> Handle(CreateEventTypeCommand request, CancellationToken cancellationToken) {
        if (request.EventType == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}");
        
        return HandleAsync(request, cancellationToken);
    }
    
    private async Task<CqrsResult<SystemEventType?>> HandleAsync(CreateEventTypeCommand request, CancellationToken cancellationToken) {
        
        SystemEventType? existed = request.EventType switch {
            DurationEventType => await _context.DurationEventTypes
                                               .FirstOrDefaultAsync(x => x.CreatedById == request.EventType.CreatedById &&
                                                                         x.Name        == request.EventType.Name,
                                                                    cancellationToken),
            OccurrenceEventType =>
                await _context.OccurrenceEventTypes
                              .FirstOrDefaultAsync(x => x.CreatedById == request.EventType.CreatedById &&
                                                        x.Name        == request.EventType.Name,
                                                   cancellationToken),
            _ => throw new UnsupportedEntityException(request.EventType.GetType())
        };
        if (existed != null) {
            if (existed.IsDeleted == true) {
                _logger.LogInformation(EfCoreLogs.CannotCreateIsDeleted, existed.GetType().Name);
                SystemEventType business = existed is DbDurationEventType
                                               ? _mapper.Map<DurationEventType>(existed)
                                               : _mapper.Map<OccurrenceEventType>(existed);
                return new CqrsResult<SystemEventType?>(business, CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogInformation(EfCoreLogs.CannotCreateIsAlreadyExisted + existed, existed.GetType().Name);
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
            await _context.SaveChangesAsync(cancellationToken);
            request.EventType.Id = db.Id;
        }

        return new CqrsResult<SystemEventType?>(request.EventType, CqrsResultCode.Created);
    }
}