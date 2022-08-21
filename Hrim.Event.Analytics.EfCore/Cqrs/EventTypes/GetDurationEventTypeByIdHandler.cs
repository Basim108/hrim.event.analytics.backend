// using AutoMapper;
// using Hrim.Event.Analytics.Abstractions.Cqrs.EventTypes;
// using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
// using MediatR;
// using Microsoft.EntityFrameworkCore;
//
// namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;
//
// public class GetDurationEventTypeByIdHandler: IRequestHandler<GetDurationEventTypeById, DurationEventType?> {
//     private readonly EventAnalyticDbContext _context;
//     private readonly IMapper                _mapper;
//
//     public GetDurationEventTypeByIdHandler(EventAnalyticDbContext context,
//                                            IMapper                mapper) {
//         _context = context;
//         _mapper  = mapper;
//     }
//
//     public Task<DurationEventType?> Handle(GetDurationEventTypeById request, CancellationToken cancellationToken) {
//         if (request.EventTypeId == default)
//             throw new ArgumentNullException(nameof(request.EventTypeId));
//         return HandleAsync(request, cancellationToken);
//     }
//
//     private async Task<DurationEventType?> HandleAsync(GetDurationEventTypeById request, CancellationToken cancellationToken) {
//         var query = _context.DurationEventTypes.AsQueryable();
//         if (request.IsNotTrackable) {
//             query.AsNoTracking();
//         }
//         var db = await query.FirstOrDefaultAsync(x => x.Id == request.EventTypeId,
//                                                  cancellationToken);
//         if (db == null)
//             return null;
//         var result = _mapper.Map<DurationEventType>(db);
//         return result;
//     }
// }