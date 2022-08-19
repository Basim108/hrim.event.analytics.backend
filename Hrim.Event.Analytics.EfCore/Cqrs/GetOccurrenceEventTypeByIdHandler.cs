using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs;

public class GetOccurrenceEventTypeByIdHandler: IRequestHandler<GetOccurrenceEventTypeById, OccurrenceEventType?> {
    private readonly EventAnalyticDbContext _context;
    private readonly IMapper                _mapper;

    public GetOccurrenceEventTypeByIdHandler(EventAnalyticDbContext context,
                                             IMapper                mapper) {
        _context = context;
        _mapper  = mapper;
    }

    public async Task<OccurrenceEventType?> Handle(GetOccurrenceEventTypeById request, CancellationToken cancellation) {
        if (request.EventTypeId == default)
            throw new ArgumentNullException(nameof(request.EventTypeId));

        var query = _context.OccurrenceEventTypes.AsQueryable();
        if (request.IsNotTrackable) {
            query.AsNoTracking();
        }
        var db = await query.FirstOrDefaultAsync(x => x.Id == request.EventTypeId,
                                                 cancellation);
        if (db == null)
            return null;
        var result = _mapper.Map<OccurrenceEventType>(db);
        return result;
    }
}