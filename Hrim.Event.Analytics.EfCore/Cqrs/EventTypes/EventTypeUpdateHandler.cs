using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class EventTypeUpdateHandler: IRequestHandler<EventTypeUpdateCommand, CqrsResult<UserEventType?>>
{
    private readonly EventAnalyticDbContext          _context;
    private readonly IMediator                       _mediator;
    private readonly ILogger<EventTypeUpdateHandler> _logger;
    private readonly IApiRequestAccessor             _requestAccessor;

    public EventTypeUpdateHandler(ILogger<EventTypeUpdateHandler> logger,
                                  EventAnalyticDbContext          context,
                                  IMediator                       mediator,
                                  IApiRequestAccessor             requestAccessor) {
        _logger          = logger;
        _context         = context;
        _mediator        = mediator;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<UserEventType?>> Handle(EventTypeUpdateCommand request, CancellationToken cancellationToken) {
        if (request.EventType == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}");
        if (request.EventType.Id == Guid.Empty)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}.{nameof(request.EventType.Id)}");
        if (request.Context == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}");

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<CqrsResult<UserEventType?>> HandleAsync(EventTypeUpdateCommand request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(messageFormat: CoreLogs.HRIM_ENTITY_ID, request.EventType.Id);
        var existed = await _context.UserEventTypes
                                    .FirstOrDefaultAsync(x => x.Id == request.EventType.Id,
                                                         cancellationToken: cancellationToken);
        if (existed == null) {
            _logger.LogDebug(message: EfCoreLogs.ENTITY_NOT_FOUND_BY_ID, nameof(UserEventType));
            return new CqrsResult<UserEventType?>(Result: null, StatusCode: CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted == true) {
            _logger.LogInformation(message: EfCoreLogs.CANNOT_UPDATE_ENTITY_IS_DELETED, existed.ConcurrentToken, nameof(UserEventType));
            return new CqrsResult<UserEventType?>(Result: existed, StatusCode: CqrsResultCode.EntityIsDeleted);
        }
        if (existed.ConcurrentToken != request.EventType.ConcurrentToken) {
            _logger.LogInformation(message: EfCoreLogs.CONCURRENT_CONFLICT,
                                   HrimOperations.Update,
                                   existed.ConcurrentToken,
                                   request.EventType.ConcurrentToken,
                                   nameof(UserEventType));
            return new CqrsResult<UserEventType?>(Result: existed, StatusCode: CqrsResultCode.Conflict);
        }
        var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
        if (existed.CreatedById != operatorUserId) {
            _logger.LogWarning(message: EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Update, existed.CreatedById, nameof(UserEventType));
            return new CqrsResult<UserEventType?>(Result: null, StatusCode: CqrsResultCode.Forbidden);
        }
        var isChanged = false;
        if (existed.Color != request.EventType.Color) {
            existed.Color = request.EventType.Color;
            isChanged     = true;
        }
        if (existed.Name != request.EventType.Name) {
            existed.Name = request.EventType.Name;
            isChanged    = true;
        }
        if (existed.IsPublic != request.EventType.IsPublic) {
            existed.IsPublic = request.EventType.IsPublic;
            isChanged        = true;
        }
        var newDescription = string.IsNullOrWhiteSpace(value: request.EventType.Description)
                                 ? null
                                 : request.EventType.Description.Trim();
        if (existed.Description != newDescription) {
            existed.Description = newDescription;
            isChanged           = true;
        }
        if (isChanged) {
            existed.UpdatedAt = DateTime.UtcNow.TruncateToMicroseconds();
            existed.ConcurrentToken++;
            if (request.SaveChanges)
                await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        }
        await UpdateAnalysisAsync(request, existed, cancellationToken);
        return new CqrsResult<UserEventType?>(Result: existed, StatusCode: CqrsResultCode.Ok);
    }

    private async Task UpdateAnalysisAsync(EventTypeUpdateCommand request, UserEventType existed, CancellationToken cancellationToken) {
        if (request.EventType.AnalysisSettings is not null && request.EventType.AnalysisSettings.Count > 0) {
            var analysisCreateResult = await _mediator.Send(new UpdateAnalysisForEventType(existed.Id,
                                                                                           request.EventType.AnalysisSettings,
                                                                                           request.Context),
                                                            cancellationToken);
            switch (analysisCreateResult.StatusCode) {
                case CqrsResultCode.Ok:
                    existed.AnalysisSettings = analysisCreateResult.Result;
                    break;
                default:
                    _logger.LogError(EfCoreLogs.WRONG_CREATE_ANALYSIS_RESPONSE,
                                     existed.Id,
                                     analysisCreateResult.StatusCode,
                                     request.EventType.AnalysisSettings);
                    break;
            }
        }
    }
}