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

public class CreateEventTypeHandler: IRequestHandler<CreateUserEventTypeCommand, CqrsResult<UserEventType?>> {
    private readonly ILogger<CreateEventTypeHandler> _logger;
    private readonly EventAnalyticDbContext          _context;

    public CreateEventTypeHandler(ILogger<CreateEventTypeHandler> logger,
                                  EventAnalyticDbContext          context) {
        _logger  = logger;
        _context = context;
    }

    public Task<CqrsResult<UserEventType?>> Handle(CreateUserEventTypeCommand request, CancellationToken cancellationToken) {
        if (request.EventType == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<UserEventType?>> HandleAsync(CreateUserEventTypeCommand request, CancellationToken cancellationToken) {
        using var eventTypeNameScope = _logger.BeginScope("EventTypeName={EventTypeName}", request.EventType.Name);
        var existed = await _context.UserEventTypes
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.CreatedById == request.EventType.CreatedById &&
                                                              x.Name        == request.EventType.Name,
                                                         cancellationToken);
        if (existed != null) {
            if (existed.IsDeleted == true) {
                _logger.LogInformation(EfCoreLogs.CannotCreateIsDeleted, nameof(UserEventType));
                return new CqrsResult<UserEventType?>(existed, CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogInformation(EfCoreLogs.CannotCreateIsAlreadyExisted + existed, nameof(UserEventType));
            return new CqrsResult<UserEventType?>(null, CqrsResultCode.Conflict);
        }
        var entityToCreate = new UserEventType() {
            Name = request.EventType.Name,
            Description = string.IsNullOrWhiteSpace(request.EventType.Description)
                              ? null
                              : request.EventType.Description.Trim(),
            Color           = request.EventType.Color,
            EventType       = request.EventType.EventType,
            IsPublic        = request.EventType.IsPublic,
            CreatedById     = request.EventType.CreatedById,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        _context.UserEventTypes.Add(entityToCreate);
        if (request.SaveChanges) {
            await _context.SaveChangesAsync(cancellationToken);
        }
        return new CqrsResult<UserEventType?>(entityToCreate, CqrsResultCode.Created);
    }
}