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
public class DurationEventCreateHandler: IRequestHandler<DurationEventCreateCommand, CqrsResult<DurationEvent?>>
{
    private readonly EventAnalyticDbContext              _context;
    private readonly ILogger<DurationEventCreateHandler> _logger;
    private readonly IMapper                             _mapper;
    private readonly IApiRequestAccessor                 _requestAccessor;

    public DurationEventCreateHandler(ILogger<DurationEventCreateHandler> logger,
                                      IMapper                             mapper,
                                      IApiRequestAccessor                 requestAccessor,
                                      EventAnalyticDbContext              context) {
        _logger          = logger;
        _mapper          = mapper;
        _requestAccessor = requestAccessor;
        _context         = context;
    }

    public Task<CqrsResult<DurationEvent?>> Handle(DurationEventCreateCommand request, CancellationToken cancellationToken) {
        if (request.EventInfo == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventInfo)}");
        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<CqrsResult<DurationEvent?>> HandleAsync(DurationEventCreateCommand request, CancellationToken cancellationToken) {
        var mappedEventInfo = _mapper.Map<DbDurationEvent>(source: request.EventInfo);
        var operatorUserId  = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
        var existed = await _context.DurationEvents
                                                  .AsNoTracking()
                                                  .FirstOrDefaultAsync(x => x.CreatedById == operatorUserId
                                                                                              && x.StartedOn   == mappedEventInfo.StartedOn
                                                                                              && x.StartedAt   == mappedEventInfo.StartedAt
                                                                                              && x.EventTypeId == mappedEventInfo.EventTypeId,
                                                                         cancellationToken: cancellationToken);
        if (existed != null) {
            if (existed.IsDeleted == true) {
                _logger.LogInformation(message: EfCoreLogs.CANNOT_CREATE_IS_DELETED, nameof(DurationEvent));
                var existedBusiness = _mapper.Map<DurationEvent>(source: existed);
                return new CqrsResult<DurationEvent?>(Result: existedBusiness, StatusCode: CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogInformation(message: EfCoreLogs.CANNOT_CREATE_IS_ALREADY_EXISTED, nameof(DurationEvent), existed.ToString());
            var info = string.Format(format: CoreLogs.ENTITY_WITH_2_PROPERTIES_ALREADY_EXISTS,
                                     nameof(DurationEvent.EventTypeId).ToSnakeCase(),
                                     nameof(DurationEvent.StartedAt).ToSnakeCase());
            return new CqrsResult<DurationEvent?>(Result: null, StatusCode: CqrsResultCode.Conflict, Info: info);
        }
        var now = DateTime.UtcNow.TruncateToMicroseconds();
        var entityToCreate = new DbDurationEvent {
            StartedOn       = mappedEventInfo.StartedOn,
            StartedAt       = mappedEventInfo.StartedAt,
            FinishedOn      = mappedEventInfo.FinishedOn,
            FinishedAt      = mappedEventInfo.FinishedAt,
            EventTypeId     = request.EventInfo.EventTypeId,
            CreatedById     = operatorUserId,
            CreatedAt       = now,
            UpdatedAt       = now,
            Props           = request.EventInfo.Props,
            ConcurrentToken = 1
        };
        _context.DurationEvents.Add(entity: entityToCreate);
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        var createdEvent = _mapper.Map<DurationEvent>(source: entityToCreate);
        return new CqrsResult<DurationEvent?>(Result: createdEvent, StatusCode: CqrsResultCode.Created);
    }
}