using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.EfCore.Extensions;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EventType = Hrim.Event.Analytics.Abstractions.Entities.EventTypes.EventType;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Entity;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class RestoreEntityCommandHandler<TEntity>: IRequestHandler<RestoreLongEntityCommand<TEntity>, CqrsResult<TEntity?>>
    where TEntity : HrimEntity<long>, new()
{
    private readonly EventAnalyticDbContext                        _context;
    private readonly ILogger<RestoreEntityCommandHandler<TEntity>> _logger;
    private readonly IMapper                                       _mapper;
    private readonly IApiRequestAccessor                           _requestAccessor;

    public RestoreEntityCommandHandler(ILogger<RestoreEntityCommandHandler<TEntity>> logger,
                                       IMapper                                       mapper,
                                       EventAnalyticDbContext                        context,
                                       IApiRequestAccessor                           requestAccessor) {
        _logger          = logger;
        _mapper          = mapper;
        _context         = context;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<TEntity?>> Handle(RestoreLongEntityCommand<TEntity> request, CancellationToken cancellationToken) {
        if (request.Id == default)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Id)}");

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<TEntity?>> HandleAsync(RestoreLongEntityCommand<TEntity> request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(messageFormat: CoreLogs.HRIM_ENTITY_ID, request.Id);
        HrimEntity<long>? existed = new TEntity() switch {
            EventType   => await _context.EventTypes.FirstOrDefaultAsync(x => x.Id   == request.Id, cancellationToken: cancellationToken),
            DurationEvent   => await _context.DurationEvents.FirstOrDefaultAsync(x => x.Id   == request.Id, cancellationToken: cancellationToken),
            OccurrenceEvent => await _context.OccurrenceEvents.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken),
            HrimTag         => await _context.HrimTags.FirstOrDefaultAsync(x => x.Id         == request.Id, cancellationToken: cancellationToken),
            HrimUser        => await _context.HrimUsers.FirstOrDefaultAsync(x => x.Id        == request.Id, cancellationToken: cancellationToken),
            _               => throw new UnsupportedEntityException(typeof(TEntity))
        };
        if (existed == null) {
            _logger.LogDebug(message: EfCoreLogs.ENTITY_NOT_FOUND_BY_ID, typeof(TEntity).Name);
            return new CqrsResult<TEntity?>(Result: null, StatusCode: CqrsResultCode.NotFound);
        }
        var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
        if (existed is IHasOwner existedOwn && existedOwn.CreatedById != operatorUserId) {
            _logger.LogWarning(message: EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Restore, existedOwn.CreatedById, typeof(TEntity).Name);
            return new CqrsResult<TEntity?>(Result: null, StatusCode: CqrsResultCode.Forbidden);
        }
        if (existed.IsDeleted != true) {
            _logger.LogDebug(message: EfCoreLogs.CANNOT_RESTORE_ENTITY_IS_NOT_DELETED, existed.ConcurrentToken, existed.GetType().Name);
            var conflictEntity = _mapper.ProjectFromDb<TEntity, long>(existed);
            return new CqrsResult<TEntity?>(Result: conflictEntity, StatusCode: CqrsResultCode.EntityIsNotDeleted);
        }
        existed.ConcurrentToken++;
        existed.UpdatedAt = DateTime.UtcNow.TruncateToMicroseconds();
        existed.IsDeleted = false;
        if (request.SaveChanges)
            await _context.SaveChangesAsync(cancellationToken: cancellationToken);
        var business = _mapper.ProjectFromDb<TEntity, long>(existed);
        return new CqrsResult<TEntity?>(Result: business, StatusCode: CqrsResultCode.Ok);
    }
}