using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class UpdateEventTypeHandler: IRequestHandler<UpdateEventTypeCommand, CqrsResult<UserEventType?>> {
    private readonly ILogger<UpdateEventTypeHandler> _logger;
    private readonly EventAnalyticDbContext          _context;

    public UpdateEventTypeHandler(ILogger<UpdateEventTypeHandler> logger,
                                  EventAnalyticDbContext          context) {
        _logger  = logger;
        _context = context;
    }

    public Task<CqrsResult<UserEventType?>> Handle(UpdateEventTypeCommand request, CancellationToken cancellationToken) {
        if (request.EventType == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}");
        if (request.EventType.Id == Guid.Empty)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}.{nameof(request.EventType.Id)}");
        if (request.Context == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}");
        if (request.Context.UserId == Guid.Empty)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}.{request.Context.UserId}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<UserEventType?>> HandleAsync(UpdateEventTypeCommand request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(CoreLogs.HRIM_ENTITY_ID, request.EventType.Id);
        var existed = await _context.UserEventTypes
                                    .FirstOrDefaultAsync(x => x.Id == request.EventType.Id,
                                                         cancellationToken);
        if (existed == null) {
            _logger.LogDebug(EfCoreLogs.ENTITY_NOT_FOUND_BY_ID, nameof(UserEventType));
            return new CqrsResult<UserEventType?>(null, CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted == true) {
            _logger.LogInformation(EfCoreLogs.CANNOT_UPDATE_ENTITY_IS_DELETED, existed.ConcurrentToken, nameof(UserEventType));
            return new CqrsResult<UserEventType?>(existed, CqrsResultCode.EntityIsDeleted);
        }
        if (existed.ConcurrentToken != request.EventType.ConcurrentToken) {
            _logger.LogInformation(EfCoreLogs.CONCURRENT_CONFLICT, 
                                   HrimOperations.Update, 
                                   existed.ConcurrentToken, 
                                   request.EventType.ConcurrentToken, 
                                   nameof(UserEventType));
            return new CqrsResult<UserEventType?>(existed, CqrsResultCode.Conflict);
        }
        if (existed.CreatedById != request.Context.UserId) {
            _logger.LogWarning(EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Update, existed.CreatedById, nameof(UserEventType));
            return new CqrsResult<UserEventType?>(null, CqrsResultCode.Forbidden);
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
        var newDescription = string.IsNullOrWhiteSpace(request.EventType.Description)
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
                await _context.SaveChangesAsync(cancellationToken);
        }
        return new CqrsResult<UserEventType?>(existed, CqrsResultCode.Ok);
    }
}