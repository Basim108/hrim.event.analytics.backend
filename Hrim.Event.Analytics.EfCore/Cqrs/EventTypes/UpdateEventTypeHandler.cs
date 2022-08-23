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
            throw new ArgumentNullException(nameof(request.EventType));
        if (request.Context == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}");
        if (request.Context.UserId == default)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}.{request.Context.UserId}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<UserEventType?>> HandleAsync(UpdateEventTypeCommand request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(CoreLogs.HrimEntityId, request.EventType.Id);
        var existed = await _context.UserEventTypes
                                    .FirstOrDefaultAsync(x => x.Id == request.EventType.Id,
                                                         cancellationToken);
        if (existed == null) {
            _logger.LogDebug(EfCoreLogs.EntityNotFoundById, nameof(UserEventType));
            return new CqrsResult<UserEventType?>(null, CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted == true) {
            _logger.LogInformation(EfCoreLogs.CannotUpdateEntityIsDeleted, existed.ConcurrentToken, nameof(UserEventType));
            return new CqrsResult<UserEventType?>(existed, CqrsResultCode.EntityIsDeleted);
        }
        if (existed.ConcurrentToken != request.EventType.ConcurrentToken) {
            _logger.LogInformation(EfCoreLogs.CannotUpdateEntityIsDeleted, existed.ConcurrentToken, nameof(UserEventType));
            return new CqrsResult<UserEventType?>(existed, CqrsResultCode.Conflict);
        }
        if (existed.CreatedById != request.Context.UserId) {
            _logger.LogInformation(EfCoreLogs.OperationIsForbiddenByUserId, existed.CreatedById, HrimOperations.Update, nameof(UserEventType));
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