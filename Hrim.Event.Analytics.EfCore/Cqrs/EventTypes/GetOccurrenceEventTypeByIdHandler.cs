// using AutoMapper;
// using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
// using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
//
// namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;
//
// public class GetOccurrenceEventTypeByIdHandler: IRequestHandler<GetOccurrenceEventTypeById, OccurrenceEventType?> {
//     private readonly EventAnalyticDbContext _context;
//     private readonly IMapper                _mapper;
//
//     public GetOccurrenceEventTypeByIdHandler(EventAnalyticDbContext context,
//                                              IMapper                mapper) {
//         _context = context;
//         _mapper  = mapper;
//     }
//
//     public Task<OccurrenceEventType?> Handle(GetOccurrenceEventTypeById request, CancellationToken cancellationToken) {
//         if (request.EventTypeId == default)
//             throw new ArgumentNullException(nameof(request.EventTypeId));
//
//         return HandleAsync(request, cancellationToken);
//     }
//     // TODO: refactor this query to a one for all entities;
//     private async Task<OccurrenceEventType?> HandleAsync(GetOccurrenceEventTypeById request, CancellationToken cancellationToken) {
//         var query = _context.OccurrenceEventTypes.AsQueryable();
//         if (request.IsNotTrackable) {
//             query.AsNoTracking();
//         }
//         var db = await query.FirstOrDefaultAsync(x => x.Id == request.EventTypeId,
//                                                  cancellationToken);
//         if (db == null)
//             return null;
//         var result = _mapper.Map<OccurrenceEventType>(db);
//         return result;
//     }
// }