using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.EfCore.DbEntities;
using Hrim.Event.Analytics.EfCore.DbEntities.Analysis;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EventType = Hrim.Event.Analytics.Abstractions.Entities.EventTypes.EventType;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class EventTypeUpdateHandler: IRequestHandler<EventTypeUpdateCommand, CqrsResult<EventType?>>
{
    private readonly EventAnalyticDbContext          _context;
    private readonly IMediator                       _mediator;
    private readonly IMapper                         _mapper;
    private readonly ILogger<EventTypeUpdateHandler> _logger;
    private readonly IApiRequestAccessor             _requestAccessor;

    public EventTypeUpdateHandler(ILogger<EventTypeUpdateHandler> logger,
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

    public Task<CqrsResult<EventType?>> Handle(EventTypeUpdateCommand request, CancellationToken cancellationToken) {
        if (request.EventType == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}");
        if (request.EventType.Id == default)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}.{nameof(request.EventType.Id)}");
        if (request.Context == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}");

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<CqrsResult<EventType?>> HandleAsync(EventTypeUpdateCommand request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(messageFormat: CoreLogs.HRIM_ENTITY_ID, request.EventType.Id);
        var dbExistedEventType = await _context.EventTypes
                                               .FirstOrDefaultAsync(x => x.Id == request.EventType.Id,
                                                                    cancellationToken: cancellationToken);
        if (dbExistedEventType == null) {
            _logger.LogDebug(message: EfCoreLogs.ENTITY_NOT_FOUND_BY_ID, nameof(EventType));
            return new CqrsResult<EventType?>(Result: null, StatusCode: CqrsResultCode.NotFound);
        }
        var blExistedEventType = _mapper.Map<EventType>(dbExistedEventType);
        if (dbExistedEventType.IsDeleted == true) {
            _logger.LogInformation(message: EfCoreLogs.CANNOT_UPDATE_ENTITY_IS_DELETED, dbExistedEventType.ConcurrentToken, nameof(EventType));
            return new CqrsResult<EventType?>(Result: blExistedEventType, StatusCode: CqrsResultCode.EntityIsDeleted);
        }
        if (dbExistedEventType.ConcurrentToken != request.EventType.ConcurrentToken) {
            _logger.LogInformation(message: EfCoreLogs.CONCURRENT_CONFLICT,
                                   HrimOperations.Update,
                                   dbExistedEventType.ConcurrentToken,
                                   request.EventType.ConcurrentToken,
                                   nameof(EventType));
            return new CqrsResult<EventType?>(Result: blExistedEventType, StatusCode: CqrsResultCode.Conflict);
        }
        var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
        if (dbExistedEventType.CreatedById != operatorUserId) {
            _logger.LogWarning(message: EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Update, dbExistedEventType.CreatedById, nameof(EventType));
            return new CqrsResult<EventType?>(Result: null, StatusCode: CqrsResultCode.Forbidden);
        }
        var isChanged = false;
        if (dbExistedEventType.ParentId != request.EventType.ParentId) {
            isChanged = true;
            DbEventType? parent = null;
            if (request.EventType.ParentId != null) {
                parent = await _context.EventTypes
                                       .FirstOrDefaultAsync(x => x.Id == request.EventType.ParentId,
                                                            cancellationToken);
                if (parent == null) {
                    _logger.LogWarning("Trying to update an event type with EventTypeParentId={EventTypeParentId} that is not existed in the storage.",
                                       request.EventType.ParentId);
                    isChanged = false;
                }
            }
            if (isChanged) {
                dbExistedEventType.Parent   = parent;
                dbExistedEventType.ParentId = parent?.Id;
                dbExistedEventType.GeneratePath();
            }
        }
        if (dbExistedEventType.Color != request.EventType.Color) {
            dbExistedEventType.Color = request.EventType.Color;
            isChanged                = true;
        }
        if (dbExistedEventType.Name != request.EventType.Name) {
            dbExistedEventType.Name = request.EventType.Name;
            isChanged               = true;
        }
        if (dbExistedEventType.IsPublic != request.EventType.IsPublic) {
            dbExistedEventType.IsPublic = request.EventType.IsPublic;
            isChanged                   = true;
        }
        var newDescription = string.IsNullOrWhiteSpace(value: request.EventType.Description)
                                 ? null
                                 : request.EventType.Description.Trim();
        if (dbExistedEventType.Description != newDescription) {
            dbExistedEventType.Description = newDescription;
            isChanged                      = true;
        }
        if (isChanged) {
            dbExistedEventType.UpdatedAt = DateTime.UtcNow.TruncateToMicroseconds();
            dbExistedEventType.ConcurrentToken++;
            if (request.SaveChanges)
                await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        }
        await UpdateAnalysisAsync(request, dbExistedEventType, cancellationToken);
        blExistedEventType = _mapper.Map<EventType>(dbExistedEventType);
        return new CqrsResult<EventType?>(Result: blExistedEventType, StatusCode: CqrsResultCode.Ok);
    }

    private async Task UpdateAnalysisAsync(EventTypeUpdateCommand request, DbEventType existed, CancellationToken cancellationToken) {
        if (request.EventType.AnalysisSettings is not null && request.EventType.AnalysisSettings.Count > 0) {
            var analysisCreateResult = await _mediator.Send(new UpdateAnalysisForEventType(existed.Id,
                                                                                                 request.EventType.AnalysisSettings,
                                                                                                 request.Context),
                                                            cancellationToken);
            switch (analysisCreateResult.StatusCode) {
                case CqrsResultCode.Ok:
                    existed.AnalysisSettings = _mapper.Map<List<DbAnalysisConfigByEventType>>(analysisCreateResult.Result);
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