using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Entity;

public class CheckEntityExistenceHandler: IRequestHandler<CheckEntityExistence, CqrsVoidResult> {
    private readonly ILogger<CheckEntityExistenceHandler> _logger;
    private readonly EventAnalyticDbContext               _context;

    public CheckEntityExistenceHandler(ILogger<CheckEntityExistenceHandler> logger,
                                       EventAnalyticDbContext               context) {
        _logger  = logger;
        _context = context;
    }

    public Task<CqrsVoidResult> Handle(CheckEntityExistence request, CancellationToken cancellationToken) {
        if (request.Id == default)
            throw new ArgumentNullException(nameof(request.Id));

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsVoidResult> HandleAsync(CheckEntityExistence request, CancellationToken cancellationToken) {
        using var   entityIdScope = _logger.BeginScope(CoreLogs.HrimEntityId, request.Id);
        HrimEntity? existed;
        switch (request.EntityType) {
            case EntityType.HrimUser:
                existed = await _context.HrimUsers
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            case EntityType.HrimTag:
                existed = await _context.HrimTags
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            case EntityType.EventType:
                existed = await _context.UserEventTypes
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            case EntityType.OccurrenceEvent:
                existed = await _context.OccurrenceEvents
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            case EntityType.DurationEvent:
                existed = await _context.DurationEvents
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (existed == null) {
            return new CqrsVoidResult(CqrsResultCode.NotFound);
        }
        if (existed.IsDeleted == true) {
            return new CqrsVoidResult(CqrsResultCode.EntityIsDeleted);
        }
        return new CqrsVoidResult(CqrsResultCode.Ok);
    }
}