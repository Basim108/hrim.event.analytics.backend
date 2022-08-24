using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.Core.Extensions;
using Hrimsoft.StringCases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class OccurrenceEventCreateHandler: IRequestHandler<OccurrenceEventCreateCommand, CqrsResult<OccurrenceEvent?>> {
    private readonly ILogger<OccurrenceEventCreateHandler> _logger;
    private readonly IMapper                               _mapper;
    private readonly IMediator                             _mediator;
    private readonly EventAnalyticDbContext                _context;

    public OccurrenceEventCreateHandler(ILogger<OccurrenceEventCreateHandler> logger,
                                        IMapper                               mapper,
                                        IMediator                             mediator,
                                        EventAnalyticDbContext                context) {
        _logger   = logger;
        _mapper   = mapper;
        _mediator = mediator;
        _context  = context;
    }

    public Task<CqrsResult<OccurrenceEvent?>> Handle(OccurrenceEventCreateCommand request, CancellationToken cancellationToken) {
        if (request.EventInfo == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventInfo)}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<OccurrenceEvent?>> HandleAsync(OccurrenceEventCreateCommand request, CancellationToken cancellationToken) {
        var mappedEventInfo = _mapper.Map<DbOccurrenceEvent>(request.EventInfo);
        var existed = await _context.OccurrenceEvents
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.CreatedById == request.Context.UserId     &&
                                                              x.OccurredOn  == mappedEventInfo.OccurredOn &&
                                                              x.OccurredAt  == mappedEventInfo.OccurredAt &&
                                                              x.EventTypeId == mappedEventInfo.EventTypeId,
                                                         cancellationToken);
        if (existed != null) {
            if (existed.IsDeleted == true) {
                _logger.LogInformation(EfCoreLogs.CANNOT_CREATE_IS_DELETED, nameof(OccurrenceEvent));
                var existedBusiness = _mapper.Map<OccurrenceEvent>(existed);
                return new CqrsResult<OccurrenceEvent?>(existedBusiness, CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogInformation(EfCoreLogs.CANNOT_CREATE_IS_ALREADY_EXISTED, nameof(OccurrenceEvent), existed.ToString());
            var info = string.Format(CoreLogs.ENTITY_WITH_2_PROPERTIES_ALREADY_EXISTS,
                                     nameof(OccurrenceEvent.EventTypeId).ToSnakeCase(),
                                     nameof(OccurrenceEvent.OccurredAt).ToSnakeCase());
            return new CqrsResult<OccurrenceEvent?>(null, CqrsResultCode.Conflict, info);
        }
        var entityToCreate = new DbOccurrenceEvent {
            OccurredOn      = mappedEventInfo.OccurredOn,
            OccurredAt      = mappedEventInfo.OccurredAt,
            EventTypeId     = request.EventInfo.EventTypeId,
            CreatedById     = request.Context.UserId,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        _context.OccurrenceEvents.Add(entityToCreate);
        if (request.SaveChanges) {
            await _context.SaveChangesAsync(cancellationToken);
        }
        var createdEvent = _mapper.Map<OccurrenceEvent>(entityToCreate);
        return new CqrsResult<OccurrenceEvent?>(createdEvent, CqrsResultCode.Created);
    }
}