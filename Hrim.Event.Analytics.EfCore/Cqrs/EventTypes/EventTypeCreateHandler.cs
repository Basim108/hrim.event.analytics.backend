using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.EfCore.DbEntities;
using Hrim.Event.Analytics.EfCore.DbEntities.Analysis;
using Hrimsoft.Core.Extensions;
using Hrimsoft.StringCases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EventType = Hrim.Event.Analytics.Abstractions.Entities.EventTypes.EventType;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class EventTypeCreateHandler: IRequestHandler<EventTypeCreateCommand, CqrsResult<EventType?>>
{
    private readonly EventAnalyticDbContext          _context;
    private readonly IMediator                       _mediator;
    private readonly IMapper                         _mapper;
    private readonly ILogger<EventTypeCreateHandler> _logger;
    private readonly IApiRequestAccessor             _requestAccessor;

    public EventTypeCreateHandler(ILogger<EventTypeCreateHandler> logger,
                                  EventAnalyticDbContext          context,
                                  IMediator                       mediator,
                                  IMapper                         mapper,
                                  IApiRequestAccessor             requestAccessor) {
        _logger          = logger;
        _context         = context;
        _mediator        = mediator;
        _mapper          = mapper;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<EventType?>> Handle(EventTypeCreateCommand request, CancellationToken cancellationToken) {
        if (request.EventType == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}");
        if (request.Context == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}");

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<CqrsResult<EventType?>> HandleAsync(EventTypeCreateCommand request, CancellationToken cancellationToken) {
        using var eventTypeNameScope = _logger.BeginScope(messageFormat: "EventTypeName={EventTypeName}", request.EventType.Name);
        var       operatorUserId     = await _requestAccessor.GetInternalUserIdAsync(cancellationToken);
        var existed = await _context.EventTypes
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.CreatedById == operatorUserId && x.Name == request.EventType.Name,
                                                         cancellationToken: cancellationToken);
        if (existed != null) {
            var result = _mapper.Map<EventType>(source: existed);
            if (existed.IsDeleted == true) {
                _logger.LogInformation(message: EfCoreLogs.CANNOT_CREATE_IS_DELETED, nameof(EventType));
                return new CqrsResult<EventType?>(result, StatusCode: CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogInformation(message: EfCoreLogs.CANNOT_CREATE_IS_ALREADY_EXISTED, nameof(EventType), existed.ToString());
            var info = string.Format(format: CoreLogs.ENTITY_WITH_PROPERTY_ALREADY_EXISTS,
                                     nameof(EventType.Name).ToSnakeCase());
            return new CqrsResult<EventType?>(Result: null, StatusCode: CqrsResultCode.Conflict, Info: info);
        }
        var now = DateTime.UtcNow.TruncateToMicroseconds();
        var entityToCreate = new DbEventType {
            Name = request.EventType.Name,
            Description = string.IsNullOrWhiteSpace(value: request.EventType.Description)
                              ? null
                              : request.EventType.Description.Trim(),
            Color           = request.EventType.Color,
            IsPublic        = request.EventType.IsPublic,
            CreatedById     = operatorUserId,
            CreatedAt       = now,
            UpdatedAt       = now,
            ConcurrentToken = 1
        };
        _context.EventTypes.Add(entity: entityToCreate);
        entityToCreate.GeneratePath();
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        if (request.EventType.AnalysisSettings is not null && request.EventType.AnalysisSettings.Count > 0) {
            var analysisCreateResult = await _mediator.Send(new UpdateAnalysisForEventType(entityToCreate.Id,
                                                                                                 request.EventType.AnalysisSettings,
                                                                                                 request.Context),
                                                            cancellationToken);
            switch (analysisCreateResult.StatusCode) {
                case CqrsResultCode.Ok:
                    entityToCreate.AnalysisSettings = _mapper.Map<List<DbAnalysisConfigByEventType>>(analysisCreateResult.Result);
                    break;
                default:
                    _logger.LogError(EfCoreLogs.WRONG_CREATE_ANALYSIS_RESPONSE,
                                     entityToCreate.Id,
                                     analysisCreateResult.StatusCode,
                                     request.EventType.AnalysisSettings);
                    break;
            }
        }
        var createdResult = _mapper.Map<EventType>(source: entityToCreate);
        return new CqrsResult<EventType?>(createdResult, StatusCode: CqrsResultCode.Created);
    }
}