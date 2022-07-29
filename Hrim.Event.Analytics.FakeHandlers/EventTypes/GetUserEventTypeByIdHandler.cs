using Hrim.Event.Analytics.Abstractions.EventTypes;
using Hrim.Event.Analytics.Models.EventTypes;
using Hrimsoft.Core.Extensions;
using MediatR;

namespace Hrim.Event.Analytics.FakeHandlers.EventTypes;

public class GetUserEventTypeByIdHandler: IRequestHandler<GetUserEventTypeById, SystemEventType> {
    public Task<SystemEventType> Handle(GetUserEventTypeById request,
                                        CancellationToken    cancellationToken) {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (request.EventTypeId == default)
            throw new ArgumentNullException(nameof(request.EventTypeId));
        
        return Task.FromResult(new OccurrenceEventType(Id: request.EventTypeId,
                                                       OccurredAt: DateTimeOffset.Now.TruncateToSeconds(),
                                                       Name: "Nice yoga practice",
                                                       Description: "",
                                                       Color: "#ff00ff",
                                                       Tags: new List<string> {
                                                           "health",
                                                           "yoga",
                                                           "positive"
                                                       },
                                                       CreatedAt: DateTime.UtcNow,
                                                       UpdateAt: DateTime.UtcNow,
                                                       IsDeleted: false,
                                                       IsPublic: true,
                                                       ConcurrentToken: 1) as SystemEventType);
    }
}