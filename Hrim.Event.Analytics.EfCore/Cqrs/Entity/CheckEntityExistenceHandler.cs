using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Entity;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Entity;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class CheckEntityExistenceHandler: IRequestHandler<CheckEntityExistence, CqrsVoidResult>
{
    private readonly EventAnalyticDbContext               _context;
    private readonly ILogger<CheckEntityExistenceHandler> _logger;

    public CheckEntityExistenceHandler(ILogger<CheckEntityExistenceHandler> logger,
                                       EventAnalyticDbContext               context) {
        _logger  = logger;
        _context = context;
    }

    public Task<CqrsVoidResult> Handle(CheckEntityExistence request, CancellationToken cancellationToken) {
        if (request.Id == default)
            throw new ArgumentNullException($"{nameof(request)}.{nameof(request.Id)}");

        return HandleAsync(request: request, cancellationToken: cancellationToken);
    }

    private async Task<CqrsVoidResult> HandleAsync(CheckEntityExistence request, CancellationToken cancellationToken) {
        using var         entityIdScope = _logger.BeginScope(messageFormat: CoreLogs.HRIM_ENTITY_ID, request.Id);
        HrimEntity<long>? existed       = null;
        switch (request.EntityType) {
            case EntityType.HrimUser:
                existed = await _context.HrimUsers
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
                break;
            case EntityType.HrimTag:
                existed = await _context.HrimTags
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
                break;
            case EntityType.EventType:
                existed = await _context.EventTypes
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
                break;
            case EntityType.OccurrenceEvent:
                existed = await _context.OccurrenceEvents
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
                break;
            case EntityType.DurationEvent:
                existed = await _context.DurationEvents
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
                break;
            default:
                throw new UnsupportedEntityException(request.EntityType.GetType());
        }
        if (existed == null)
            return new CqrsVoidResult(StatusCode: CqrsResultCode.NotFound);
        if (existed.IsDeleted == true)
            return new CqrsVoidResult(StatusCode: CqrsResultCode.EntityIsDeleted);
        return new CqrsVoidResult(StatusCode: CqrsResultCode.Ok);
    }
}