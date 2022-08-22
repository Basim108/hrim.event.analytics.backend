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
        existed.ConcurrentToken++;
        existed.Color    = request.EventType.Color;
        existed.Name     = request.EventType.Name;
        existed.IsPublic = request.EventType.IsPublic;
        existed.Description = string.IsNullOrWhiteSpace(request.EventType.Description)
                                  ? null
                                  : request.EventType.Description;
        existed.UpdatedAt = DateTime.UtcNow.TruncateToMicroseconds();
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellationToken);
        return new CqrsResult<UserEventType?>(existed, CqrsResultCode.Ok);
    }
}