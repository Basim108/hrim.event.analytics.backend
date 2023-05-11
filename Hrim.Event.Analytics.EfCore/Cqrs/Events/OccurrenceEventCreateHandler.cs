using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.Core.Extensions;
using Hrimsoft.StringCases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class OccurrenceEventCreateHandler: IRequestHandler<OccurrenceEventCreateCommand, CqrsResult<OccurrenceEvent?>>
{
    private readonly EventAnalyticDbContext                _context;
    private readonly ILogger<OccurrenceEventCreateHandler> _logger;
    private readonly IMapper                               _mapper;
    private readonly IApiRequestAccessor                   _requestAccessor;

    public OccurrenceEventCreateHandler(ILogger<OccurrenceEventCreateHandler> logger,
                                        IMapper                               mapper,
                                        EventAnalyticDbContext                context,
                                        IApiRequestAccessor                   requestAccessor) {
        _logger          = logger;
        _mapper          = mapper;
        _context         = context;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<OccurrenceEvent?>> Handle(OccurrenceEventCreateCommand request, CancellationToken cancellationToken) {
        if (request.EventInfo == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventInfo)}");

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<CqrsResult<OccurrenceEvent?>> HandleAsync(OccurrenceEventCreateCommand request, CancellationToken cancellationToken) {
        var mappedEventInfo = _mapper.Map<DbOccurrenceEvent>(source: request.EventInfo);
        var operatorUserId  = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
        var existed = await _context.OccurrenceEvents
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.CreatedById == operatorUserId
                                                           && x.OccurredOn  == mappedEventInfo.OccurredOn
                                                           && x.OccurredAt  == mappedEventInfo.OccurredAt
                                                           && x.EventTypeId == mappedEventInfo.EventTypeId,
                                                         cancellationToken: cancellationToken);
        if (existed != null) {
            if (existed.IsDeleted == true) {
                _logger.LogInformation(message: EfCoreLogs.CANNOT_CREATE_IS_DELETED, nameof(OccurrenceEvent));
                var existedBusiness = _mapper.Map<OccurrenceEvent>(source: existed);
                return new CqrsResult<OccurrenceEvent?>(Result: existedBusiness, StatusCode: CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogInformation(message: EfCoreLogs.CANNOT_CREATE_IS_ALREADY_EXISTED, nameof(OccurrenceEvent), existed.ToString());
            var info = string.Format(format: CoreLogs.ENTITY_WITH_2_PROPERTIES_ALREADY_EXISTS,
                                     nameof(OccurrenceEvent.EventTypeId).ToSnakeCase(),
                                     nameof(OccurrenceEvent.OccurredAt).ToSnakeCase());
            return new CqrsResult<OccurrenceEvent?>(Result: null, StatusCode: CqrsResultCode.Conflict, Info: info);
        }
        var entityToCreate = new DbOccurrenceEvent {
            OccurredOn      = mappedEventInfo.OccurredOn,
            OccurredAt      = mappedEventInfo.OccurredAt,
            EventTypeId     = request.EventInfo.EventTypeId,
            CreatedById     = operatorUserId,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        _context.OccurrenceEvents.Add(entity: entityToCreate);
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        var createdEvent = _mapper.Map<OccurrenceEvent>(source: entityToCreate);
        return new CqrsResult<OccurrenceEvent?>(Result: createdEvent, StatusCode: CqrsResultCode.Created);
    }
}