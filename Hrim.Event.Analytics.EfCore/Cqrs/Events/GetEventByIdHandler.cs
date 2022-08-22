using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Cqrs.Events;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Events;

public class GetEventByIdHandler<TEvent>: IRequestHandler<GetEventById<TEvent>, TEvent?>
    where TEvent : BaseEvent, new() {
    private readonly EventAnalyticDbContext _context;
    private readonly IMapper                _mapper;

    public GetEventByIdHandler(EventAnalyticDbContext context,
                               IMapper                mapper) {
        _context = context;
        _mapper  = mapper;
    }

    public Task<TEvent?> Handle(GetEventById<TEvent> request, CancellationToken cancellationToken) {
        if (request.Id == default)
            throw new ArgumentNullException(nameof(request.Id));
        return HandleAsync(request, cancellationToken);
    }

    private async Task<TEvent?> HandleAsync(GetEventById<TEvent> request, CancellationToken cancellationToken) {
        BaseEvent? db;
        switch (new TEvent()) {
            case DurationEvent:
                var durationQuery = _context.DurationEvents.AsQueryable();
                if (request.IsNotTrackable) {
                    durationQuery = durationQuery.AsNoTracking();
                }
                db = await durationQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            case OccurrenceEvent:
                var occurrenceQuery = _context.OccurrenceEvents.AsQueryable();
                if (request.IsNotTrackable) {
                    occurrenceQuery = occurrenceQuery.AsNoTracking();
                }
                db = await occurrenceQuery.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            default:
                throw new UnsupportedEntityException(typeof(TEvent));
        }
        if (db == null)
            return null;
        var result = _mapper.Map<TEvent>(db);
        return result;
    }
}