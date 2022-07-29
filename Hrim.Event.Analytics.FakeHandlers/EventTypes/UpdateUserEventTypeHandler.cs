using Hrim.Event.Analytics.Abstractions.EventTypes;
using Hrim.Event.Analytics.Models.EventTypes;
using MediatR;

namespace Hrim.Event.Analytics.FakeHandlers.EventTypes; 

public class UpdateUserEventTypeHandler: IRequestHandler<UpdateUserEventType, SystemEventType> {
    public Task<SystemEventType> Handle(UpdateUserEventType request, CancellationToken cancellationToken) {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (request.EventType == null)
            throw new ArgumentNullException(nameof(request.EventType));

        var newEntity       = request.EventType with {
            UpdateAt        = DateTime.UtcNow,
            ConcurrentToken = request.EventType.ConcurrentToken + 1
        };
        return Task.FromResult(newEntity);
    }
}
