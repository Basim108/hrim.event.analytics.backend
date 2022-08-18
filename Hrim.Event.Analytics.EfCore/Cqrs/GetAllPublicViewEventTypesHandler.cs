using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.ViewModels.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs;

public class GetAllPublicViewEventTypesHandler: IRequestHandler<GetAllPublicViewEventTypes, IEnumerable<ViewSystemEventType>> {
    private readonly EventAnalyticDbContext _context;

    public GetAllPublicViewEventTypesHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public async Task<IEnumerable<ViewSystemEventType>> Handle(GetAllPublicViewEventTypes request, CancellationToken cancellation) {
        var durationTypes = await _context.DurationEventTypes
                                          .Where(x => x.IsPublic  == true &&
                                                      x.IsDeleted == true)
                                          .AsNoTracking()
                                          .Select(x => new ViewDurationEventType(x.Id,
                                                                                 x.StartedAt,
                                                                                 x.FinishedAt,
                                                                                 x.Name,
                                                                                 x.Description,
                                                                                 x.Color,
                                                                                 x.IsPublic))
                                          .Cast<ViewSystemEventType>()
                                          .ToListAsync(cancellation);
        var occurrenceTypes = await _context.OccurrenceEventTypes
                                            .Where(x => x.IsPublic  == true &&
                                                        x.IsDeleted == true)
                                            .AsNoTracking()
                                            .Select(x => new ViewOccurrenceEventType(x.Id,
                                                                                     x.OccurredAt,
                                                                                     x.Name,
                                                                                     x.Description,
                                                                                     x.Color,
                                                                                     x.IsPublic))
                                            .Cast<ViewSystemEventType>()
                                            .ToListAsync(cancellation);
        var result = new List<ViewSystemEventType>(durationTypes.Count + occurrenceTypes.Count);
        result.AddRange(durationTypes);
        result.AddRange(occurrenceTypes);
        return result;
    }
}