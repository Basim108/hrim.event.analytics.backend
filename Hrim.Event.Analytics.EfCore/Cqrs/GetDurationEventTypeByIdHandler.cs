using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs;

public class GetDurationEventTypeByIdHandler: IRequestHandler<GetDurationEventTypeById, DurationEventType?> {
    private readonly EventAnalyticDbContext _context;
    private readonly IMapper                _mapper;

    public GetDurationEventTypeByIdHandler(EventAnalyticDbContext context,
                                           IMapper                mapper) {
        _context = context;
        _mapper  = mapper;
    }

    public async Task<DurationEventType?> Handle(GetDurationEventTypeById request, CancellationToken cancellation) {
        if (request.EventTypeId == default)
            throw new ArgumentNullException(nameof(request.EventTypeId));

        var query = _context.DurationEventTypes.AsQueryable();
        if (request.IsNotTrackable) {
            query.AsNoTracking();
        }
        var db = await query.FirstOrDefaultAsync(x => x.Id == request.EventTypeId,
                                           cancellation);
        if (db == null)
            return null;
        var result = _mapper.Map<DurationEventType>(db);
        return result;
    }
}