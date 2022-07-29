using Hrim.Event.Analytics.Abstractions.EventTypes;
using Hrim.Event.Analytics.Models.EventTypes;
using Hrimsoft.Core.Extensions;
using MediatR;

namespace Hrim.Event.Analytics.FakeHandlers.EventTypes;

public class GetAllPublicUserEventTypesHandler: IRequestHandler<GetAllPublicUserEventTypes, IEnumerable<SystemEventType>> {
    public Task<IEnumerable<SystemEventType>> Handle(GetAllPublicUserEventTypes request, 
                                                     CancellationToken cancellationToken) {
        return Task.FromResult(new List<SystemEventType> {
            new OccurrenceEventType(Id: Guid.NewGuid(),
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
                                    ConcurrentToken: 1),
            new DurationEventType(Id: Guid.NewGuid(),
                                  StartedAt: DateTimeOffset.Now.AddHours(-2).TruncateToSeconds(),
                                  FinishedAt: DateTimeOffset.Now.TruncateToSeconds(),
                                  Name: "Headeache",
                                  Description: "",
                                  Color: "#ff0000",
                                  Tags: new List<string> {
                                      "health",
                                      "negative"
                                  },
                                  CreatedAt: DateTime.UtcNow,
                                  UpdateAt: DateTime.UtcNow,
                                  IsDeleted: false,
                                  IsPublic: true,
                                  ConcurrentToken: 1)
        } as IEnumerable<SystemEventType>);
    }
}