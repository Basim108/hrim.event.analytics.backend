using AutoMapper;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;
using Hrim.Event.Analytics.EfCore.Extensions;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs;

public class RestoreEntityCommandHandler<TEntity>: IRequestHandler<RestoreEntityCommand<TEntity>, CqrsResult<TEntity?>>
    where TEntity : Entity, new() {
    private readonly ILogger<RestoreEntityCommandHandler<TEntity>> _logger;
    private readonly IMapper                                       _mapper;
    private readonly EventAnalyticDbContext                        _context;

    public RestoreEntityCommandHandler(ILogger<RestoreEntityCommandHandler<TEntity>> logger,
                                       IMapper                                       mapper,
                                       EventAnalyticDbContext                        context) {
        _logger  = logger;
        _mapper  = mapper;
        _context = context;
    }

    public Task<CqrsResult<TEntity?>> Handle(RestoreEntityCommand<TEntity> request, CancellationToken cancellationToken) {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (request.Id == default)
            throw new ArgumentNullException(nameof(request.Id));

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<TEntity?>> HandleAsync(RestoreEntityCommand<TEntity> request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(CoreLogs.HrimEntityId, request.Id);
        Entity? existed = new TEntity() switch {
            SystemEventType => await _context.UserEventTypes.FirstOrDefaultAsync(x => x.Id   == request.Id, cancellationToken),
            DurationEvent   => await _context.DurationEvents.FirstOrDefaultAsync(x => x.Id   == request.Id, cancellationToken),
            OccurrenceEvent => await _context.OccurrenceEvents.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken),
            HrimTag         => await _context.HrimTags.FirstOrDefaultAsync(x => x.Id         == request.Id, cancellationToken),
            HrimUser        => await _context.HrimUsers.FirstOrDefaultAsync(x => x.Id        == request.Id, cancellationToken),
            _               => null
        };
        if (existed == null) {
            _logger.LogDebug(EfCoreLogs.EntityNotFoundById, typeof(TEntity).Name);
            return new CqrsResult<TEntity?>(null, CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted != true) {
            _logger.LogDebug(EfCoreLogs.CannotRestoreEntityIsNotDeleted, existed.ConcurrentToken, existed.GetType().Name);
            var conflictEntity = _mapper.ProjectFromDb<TEntity>(existed);
            return new CqrsResult<TEntity?>(conflictEntity, CqrsResultCode.EntityIsNotDeleted);
        }
        existed.ConcurrentToken++;
        existed.UpdatedAt = DateTime.UtcNow.TruncateToMicroseconds();
        existed.IsDeleted = false;
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellationToken);
        var business = _mapper.ProjectFromDb<TEntity>(existed);
        return new CqrsResult<TEntity?>(business, CqrsResultCode.Ok);
    }
}