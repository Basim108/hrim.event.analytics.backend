using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

public class CreateOccurrenceEventHandler: IRequestHandler<CreateOccurrenceEventCommand, CqrsResult<OccurrenceEvent?>> {
    private readonly ILogger<CreateOccurrenceEventHandler> _logger;
    private readonly IMapper                               _mapper;
    private readonly IMediator                             _mediator;
    private readonly EventAnalyticDbContext                _context;

    public CreateOccurrenceEventHandler(ILogger<CreateOccurrenceEventHandler> logger,
                                        IMapper                               mapper,
                                        IMediator                             mediator,
                                        EventAnalyticDbContext                context) {
        _logger   = logger;
        _mapper   = mapper;
        _mediator = mediator;
        _context  = context;
    }

    public Task<CqrsResult<OccurrenceEvent?>> Handle(CreateOccurrenceEventCommand request, CancellationToken cancellationToken) {
        if (request.EventInfo == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventInfo)}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<OccurrenceEvent?>> HandleAsync(CreateOccurrenceEventCommand request, CancellationToken cancellationToken) {
        var mappedEventInfo = _mapper.Map<DbOccurrenceEvent>(request.EventInfo);
        var existed = await _context.OccurrenceEvents
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.CreatedById == request.EventInfo.CreatedById &&
                                                              x.OccurredOn  == mappedEventInfo.OccurredOn    &&
                                                              x.OccurredAt  == mappedEventInfo.OccurredAt    &&
                                                              x.EventTypeId == mappedEventInfo.EventTypeId   &&
                                                              x.IsDeleted   != true,
                                                         cancellationToken);
        if (existed != null) {
            if (existed.IsDeleted == true) {
                _logger.LogInformation(EfCoreLogs.CannotCreateIsDeleted, nameof(OccurrenceEvent));
                var existedBusiness = _mapper.Map<OccurrenceEvent>(existed);
                return new CqrsResult<OccurrenceEvent?>(existedBusiness, CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogInformation(EfCoreLogs.CannotCreateIsAlreadyExisted + existed, nameof(OccurrenceEvent));
            return new CqrsResult<OccurrenceEvent?>(null, CqrsResultCode.Conflict);
        }
        var isUserExists = await _mediator.Send(new CheckUserExistence(request.EventInfo.CreatedById, request.CorrelationId),
                                                cancellationToken);
        if(isUserExists.StatusCode != CqrsResultCode.Ok) {
            return new CqrsResult<OccurrenceEvent?>(null, CqrsResultCode.BadRequest, "User who set as an owner of the event does not exist");
        }
        var entityToCreate = new DbOccurrenceEvent {
            OccurredOn      = mappedEventInfo.OccurredOn,
            OccurredAt      = mappedEventInfo.OccurredAt,
            EventTypeId     = request.EventInfo.EventTypeId,
            CreatedById     = request.EventInfo.CreatedById,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        _context.OccurrenceEvents.Add(entityToCreate);
        if (request.SaveChanges) {
            await _context.SaveChangesAsync(cancellationToken);
        }
        var savedBusiness = _mapper.Map<OccurrenceEvent>(entityToCreate);
        return new CqrsResult<OccurrenceEvent?>(savedBusiness, CqrsResultCode.Created);
    }
}