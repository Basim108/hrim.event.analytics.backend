using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs;

public class SoftDeleteEntityCommandHandler<TEntity>: IRequestHandler<SoftDeleteEntityCommand<TEntity>, CqrsResult<TEntity?>>
    where TEntity : Entity, new() {
    private readonly EventAnalyticDbContext _context;

    public SoftDeleteEntityCommandHandler(EventAnalyticDbContext context) {
        _context = context;
    }

    public async Task<CqrsResult<TEntity?>> Handle(SoftDeleteEntityCommand<TEntity> request, CancellationToken cancellation) {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (request.Id == default)
            throw new ArgumentNullException(nameof(request.Id));

        Entity? existed = new TEntity() switch {
            DurationEventType   => await _context.DurationEventTypes.FirstOrDefaultAsync(x => x.Id   == request.Id, cancellation),
            OccurrenceEventType => await _context.OccurrenceEventTypes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellation),
            HrimTag             => await _context.HrimTags.FirstOrDefaultAsync(x => x.Id             == request.Id, cancellation),
            HrimUser            => await _context.HrimUsers.FirstOrDefaultAsync(x => x.Id            == request.Id, cancellation),
            _                   => null
        };
        if (existed == null) {
            return new CqrsResult<TEntity?>(null, CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted == true) {
            return new CqrsResult<TEntity?>(existed as TEntity, CqrsResultCode.EntityIsDeleted);
        }
        existed.UpdatedAt = DateTime.UtcNow.TruncateToMicroseconds();
        existed.IsDeleted = true;
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellation);
        return new CqrsResult<TEntity?>(existed as TEntity, CqrsResultCode.Ok);
    }
}