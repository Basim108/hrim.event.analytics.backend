using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrimsoft.Core.Extensions;
using Hrimsoft.StringCases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class EventTypeCreateHandler: IRequestHandler<EventTypeCreateCommand, CqrsResult<UserEventType?>>
{
    private readonly EventAnalyticDbContext          _context;
    private readonly IMediator                       _mediator;
    private readonly ILogger<EventTypeCreateHandler> _logger;
    private readonly IApiRequestAccessor             _requestAccessor;

    public EventTypeCreateHandler(ILogger<EventTypeCreateHandler> logger,
                                  EventAnalyticDbContext          context,
                                  IMediator                       mediator,
                                  IApiRequestAccessor             requestAccessor) {
        _logger          = logger;
        _context         = context;
        _mediator        = mediator;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<UserEventType?>> Handle(EventTypeCreateCommand request, CancellationToken cancellationToken) {
        if (request.EventType == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}");
        if (request.Context == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}");

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<CqrsResult<UserEventType?>> HandleAsync(EventTypeCreateCommand request, CancellationToken cancellationToken) {
        using var eventTypeNameScope = _logger.BeginScope(messageFormat: "EventTypeName={EventTypeName}", request.EventType.Name);
        var       operatorUserId     = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
        var existed = await _context.UserEventTypes
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.CreatedById == operatorUserId && x.Name == request.EventType.Name,
                                                         cancellationToken: cancellationToken);
        if (existed != null) {
            if (existed.IsDeleted == true) {
                _logger.LogInformation(message: EfCoreLogs.CANNOT_CREATE_IS_DELETED, nameof(UserEventType));
                return new CqrsResult<UserEventType?>(Result: existed, StatusCode: CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogInformation(message: EfCoreLogs.CANNOT_CREATE_IS_ALREADY_EXISTED, nameof(UserEventType), existed.ToString());
            var info = string.Format(format: CoreLogs.ENTITY_WITH_PROPERTY_ALREADY_EXISTS,
                                     nameof(UserEventType.Name).ToSnakeCase());
            return new CqrsResult<UserEventType?>(Result: null, StatusCode: CqrsResultCode.Conflict, Info: info);
        }
        var entityToCreate = new UserEventType {
            Name = request.EventType.Name,
            Description = string.IsNullOrWhiteSpace(value: request.EventType.Description)
                              ? null
                              : request.EventType.Description.Trim(),
            Color           = request.EventType.Color,
            IsPublic        = request.EventType.IsPublic,
            CreatedById     = operatorUserId,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        _context.UserEventTypes.Add(entity: entityToCreate);
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        if (request.EventType.AnalysisSettings is not null && request.EventType.AnalysisSettings.Count > 0) {
            var analysisCreateResult = await _mediator.Send(new UpdateAnalysisForEventType(entityToCreate.Id, 
                                                                                           request.EventType.AnalysisSettings,
                                                                                           request.Context),
                                                            cancellationToken);
            switch (analysisCreateResult.StatusCode) {
                case CqrsResultCode.Ok:
                    entityToCreate.AnalysisSettings = analysisCreateResult.Result;
                    break;
                default:
                    _logger.LogError(EfCoreLogs.WRONG_CREATE_ANALYSIS_RESPONSE, 
                                     entityToCreate.Id,
                                     analysisCreateResult.StatusCode,
                                     request.EventType.AnalysisSettings);
                    break;
            }
        }
        return new CqrsResult<UserEventType?>(Result: entityToCreate, StatusCode: CqrsResultCode.Created);
    }
}