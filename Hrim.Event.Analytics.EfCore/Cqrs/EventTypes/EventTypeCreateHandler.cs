using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrimsoft.Core.Extensions;
using Hrimsoft.StringCases;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

[SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
public class EventTypeCreateHandler: IRequestHandler<EventTypeCreateCommand, CqrsResult<UserEventType?>> {
    private readonly ILogger<EventTypeCreateHandler> _logger;
    private readonly EventAnalyticDbContext          _context;

    public EventTypeCreateHandler(ILogger<EventTypeCreateHandler> logger,
                                  EventAnalyticDbContext          context) {
        _logger  = logger;
        _context = context;
    }

    public Task<CqrsResult<UserEventType?>> Handle(EventTypeCreateCommand request, CancellationToken cancellationToken) {
        if (request.EventType == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.EventType)}");
        if (request.Context == null)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}");
        if (request.Context.UserId == Guid.Empty)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Context)}.{request.Context.UserId}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<UserEventType?>> HandleAsync(EventTypeCreateCommand request, CancellationToken cancellationToken) {
        using var eventTypeNameScope = _logger.BeginScope("EventTypeName={EventTypeName}", request.EventType.Name);
        var existed = await _context.UserEventTypes
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.CreatedById == request.Context.UserId &&
                                                              x.Name        == request.EventType.Name,
                                                         cancellationToken);
        if (existed != null) {
            if (existed.IsDeleted == true) {
                _logger.LogInformation(EfCoreLogs.CANNOT_CREATE_IS_DELETED, nameof(UserEventType));
                return new CqrsResult<UserEventType?>(existed, CqrsResultCode.EntityIsDeleted);
            }
            _logger.LogInformation(EfCoreLogs.CANNOT_CREATE_IS_ALREADY_EXISTED, nameof(UserEventType), existed.ToString());
            var info = string.Format(CoreLogs.ENTITY_WITH_PROPERTY_ALREADY_EXISTS,
                                     nameof(UserEventType.Name).ToSnakeCase());
            return new CqrsResult<UserEventType?>(null, CqrsResultCode.Conflict, info);
        }
        var entityToCreate = new UserEventType {
            Name = request.EventType.Name,
            Description = string.IsNullOrWhiteSpace(request.EventType.Description)
                              ? null
                              : request.EventType.Description.Trim(),
            Color           = request.EventType.Color,
            IsPublic        = request.EventType.IsPublic,
            CreatedById     = request.Context.UserId,
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