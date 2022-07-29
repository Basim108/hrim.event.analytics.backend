using Hrim.Event.Analytics.Abstractions.EventTypes;
using Hrim.Event.Analytics.Models.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.FakeHandlers.EventTypes; 

public class CreateUserEventTypeHandler: IRequestHandler<CreateUserEventType, SystemEventType> {
    public Task<SystemEventType> Handle(CreateUserEventType request, CancellationToken cancellationToken) {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (request.EventType == null)
            throw new ArgumentNullException(nameof(request.EventType));

        var newEntity       = request.EventType with {
            Id              = Guid.NewGuid(),
            CreatedAt       = DateTime.UtcNow,
            UpdateAt        = null,
            ConcurrentToken = 1
        };
        return Task.FromResult(newEntity);
    }
}
