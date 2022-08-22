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

public class DurationEventCreateHandler: IRequestHandler<DurationEventCreateCommand, CqrsResult<DurationEvent?>> {
    private readonly ILogger<DurationEventCreateHandler> _logger;
    private readonly IMapper                             _mapper;
    private readonly IMediator                           _mediator;
    private readonly EventAnalyticDbContext              _context;

    public DurationEventCreateHandler(ILogger<DurationEventCreateHandler> logger,
                                      IMapper                             mapper,
                                      IMediator                           mediator,
                                      EventAnalyticDbContext              context) {
        _logger   = logger;
        _mapper   = mapper;
        _mediator = mediator;
        _context  = context;
    }

    public Task<CqrsResult<DurationEvent?>> Handle(DurationEventCreateCommand request, CancellationToken cancellationToken) {
        if (request.EventInfo == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventInfo)}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<DurationEvent?>> HandleAsync(DurationEventCreateCommand request, CancellationToken cancellationToken) {
        var mappedEventInfo = _mapper.Map<DbDurationEvent>(request.EventInfo);
        var existed = await _context.DurationEvents
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.CreatedById == request.EventInfo.CreatedById &&
                                                              x.StartedOn   == mappedEventInfo.StartedOn     &&
                                                              x.StartedAt   == mappedEventInfo.StartedAt     &&
                                                              x.EventTypeId == mappedEventInfo.EventTypeId,
                                                         cancellationToken);
        if (existed != null) {
            if (existed.IsDeleted == true) {
                _logger.LogInformation(EfCoreLogs.CannotCreateIsDeleted, nameof(DurationEvent));
                var existedBusiness = _mapper.Map<DurationEvent>(existed);
                return new CqrsResult<DurationEvent?>(existedBusiness, CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogInformation(EfCoreLogs.CannotCreateIsAlreadyExisted + existed, nameof(DurationEvent));
            return new CqrsResult<DurationEvent?>(null, CqrsResultCode.Conflict);
        }
        // TODO: [refactoring]: move check to fluent validation so it'll return wrong field_name and info
        var isUserExists = await _mediator.Send(new CheckUserExistence(request.EventInfo.CreatedById, request.CorrelationId),
                                                cancellationToken);
        if (isUserExists.StatusCode != CqrsResultCode.Ok) {
            return new CqrsResult<DurationEvent?>(null, CqrsResultCode.BadRequest, "User who set as an owner of the event does not exist");
        }
        var entityToCreate = new DbDurationEvent() {
            StartedOn       = mappedEventInfo.StartedOn,
            StartedAt       = mappedEventInfo.StartedAt,
            FinishedOn      = mappedEventInfo.FinishedOn,
            FinishedAt      = mappedEventInfo.FinishedAt,
            EventTypeId     = request.EventInfo.EventTypeId,
            CreatedById     = request.EventInfo.CreatedById,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        _context.DurationEvents.Add(entityToCreate);
        if (request.SaveChanges) {
            await _context.SaveChangesAsync(cancellationToken);
        }
        var createdEvent = _mapper.Map<DurationEvent>(entityToCreate);
        return new CqrsResult<DurationEvent?>(createdEvent, CqrsResultCode.Created);
    }
}