using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.EventTypes;

public class SoftDeleteEntityCommandHandler<TEntity>: IRequestHandler<SoftDeleteEntityCommand<TEntity>, CqrsResult<TEntity?>>
    where TEntity : Entity, new() {
    private readonly ILogger<SoftDeleteEntityCommandHandler<TEntity>> _logger;
    private readonly IMapper                                          _mapper;
    private readonly EventAnalyticDbContext                           _context;

    public SoftDeleteEntityCommandHandler(ILogger<SoftDeleteEntityCommandHandler<TEntity>> logger,
                                          IMapper                                          mapper,
                                          EventAnalyticDbContext                           context) {
        _logger  = logger;
        _mapper  = mapper;
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
            _logger.LogDebug(EfCoreLogs.EntityNotFoundById, request.Id, nameof(OccurrenceEventType));
            return new CqrsResult<TEntity?>(null, CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted == true) {
            _logger.LogDebug(EfCoreLogs.CannotUpdateEntityIsDeleted, request.Id, existed.ConcurrentToken, existed.GetType().Name);
            var business = existed switch {
                DbDurationEventType   => _mapper.Map<TEntity>(existed),
                DbOccurrenceEventType => _mapper.Map<TEntity>(existed),
                _                     => existed
            };
            return new CqrsResult<TEntity?>(business as TEntity, CqrsResultCode.EntityIsDeleted);
        }
        existed.UpdatedAt = DateTime.UtcNow.TruncateToMicroseconds();
        existed.IsDeleted = true;
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellation);
        return new CqrsResult<TEntity?>(existed as TEntity, CqrsResultCode.Ok);
    }
}