using System.Diagnostics.CodeAnalysis;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Analysis;

[SuppressMessage(category: "Usage", checkId: "CA2208:Instantiate argument exceptions correctly")]
public class UpdateAnalysisForEventTypeHandler: IRequestHandler<UpdateAnalysisForEventType, CqrsResult<List<AnalysisByEventType>?>>
{
    private readonly EventAnalyticDbContext                     _context;
    private readonly ILogger<UpdateAnalysisForEventTypeHandler> _logger;
    private readonly IApiRequestAccessor                        _requestAccessor;

    public UpdateAnalysisForEventTypeHandler(EventAnalyticDbContext                     context,
                                             ILogger<UpdateAnalysisForEventTypeHandler> logger,
                                             IApiRequestAccessor                        requestAccessor) {
        _context         = context;
        _logger          = logger;
        _requestAccessor = requestAccessor;
    }

    public Task<CqrsResult<List<AnalysisByEventType>?>> Handle(UpdateAnalysisForEventType request, CancellationToken cancellationToken) {
        if (request.EventTypeId == Guid.Empty)
            throw new ArgumentNullException(nameof(request), nameof(request.EventTypeId));

        return HandleAsync(request, cancellationToken);
    }

    private async Task<CqrsResult<List<AnalysisByEventType>?>> HandleAsync(UpdateAnalysisForEventType request, CancellationToken cancellationToken) {
        using var entityIdScope = _logger.BeginScope(messageFormat: CoreLogs.HRIM_ENTITY_ID, request.EventTypeId);
        try {
            var result = await _context.UserEventTypes.FirstOrDefaultAsync(x => x.Id == request.EventTypeId, cancellationToken);
            if (result == null)
                return new CqrsResult<List<AnalysisByEventType>?>(Result: null, StatusCode: CqrsResultCode.NotFound);
            var operatorUserId = await _requestAccessor.GetInternalUserIdAsync(cancellation: cancellationToken);
            if (result.CreatedById != operatorUserId) {
                _logger.LogWarning(message: EfCoreLogs.OPERATION_IS_FORBIDDEN_FOR_USER_ID, HrimOperations.Read, result.CreatedById, nameof(AnalysisByEventType));
                return new CqrsResult<List<AnalysisByEventType>?>(Result: null, StatusCode: CqrsResultCode.Forbidden);
            }
            var settings = await _context.AnalysisByEventType
                                         .Where(x => x.EventTypeId == request.EventTypeId)
                                         .ToListAsync(cancellationToken);
            var resultList       = new List<AnalysisByEventType>(request.Analysis.Count);
            var hasGlobalChanges = false;
            foreach (var incoming in request.Analysis) {
                var db = settings.FirstOrDefault(x => x.AnalysisCode == incoming.AnalysisCode);
                if (db is null) {
                    db               = CreateAnalysis(request.EventTypeId, incoming);
                    hasGlobalChanges = true;
                }
                else {
                    if (db.ConcurrentToken != incoming.ConcurrentToken)
                        return new CqrsResult<List<AnalysisByEventType>?>(Result: request.Analysis, StatusCode: CqrsResultCode.Conflict);
                    var hasLocalChanges = CheckDifferences(db, incoming);
                    if (hasLocalChanges) {
                        UpdateStoredAnalysis(db, incoming);
                        hasGlobalChanges = true;
                    }
                }
                resultList.Add(db);
            }
            if (hasGlobalChanges) {
                await _context.SaveChangesAsync(cancellationToken);
            }
            return new CqrsResult<List<AnalysisByEventType>?>(Result: resultList, StatusCode: CqrsResultCode.Ok);
        }
        catch (TimeoutException ex) {
            _logger.LogWarning(message: EfCoreLogs.OPERATION_TIMEOUT, HrimOperations.Read, ex.Message);
            return new CqrsResult<List<AnalysisByEventType>?>(Result: null, StatusCode: CqrsResultCode.Locked);
        }
    }

    /// <summary>
    /// Check differences between db analysis and the incoming one.
    /// Ignores additional settings in incoming model as not trusted.
    /// </summary>
    /// <returns>Returns True when there are changes that have to be updated in the storage</returns>
    public static bool CheckDifferences(AnalysisByEventType db, AnalysisByEventType incoming) {
        var hasChanges = db.IsOn != incoming.IsOn || db.Settings == null && incoming.Settings != null || db.Settings != null && incoming.Settings == null;
        if (!hasChanges && db.Settings != null && incoming.Settings != null) {
            foreach (var dbPair in db.Settings) {
                hasChanges = !incoming.Settings.ContainsKey(dbPair.Key) || incoming.Settings[dbPair.Key] != dbPair.Value;
            }
        }
        return hasChanges;
    }

    private AnalysisByEventType CreateAnalysis(Guid eventTypeId, AnalysisByEventType incomingTemplate) {
        var db = new AnalysisByEventType {
            EventTypeId     = eventTypeId,
            AnalysisCode    = incomingTemplate.AnalysisCode,
            IsOn            = incomingTemplate.IsOn,
            Settings        = incomingTemplate.Settings,
            CreatedAt       = DateTime.UtcNow,
            ConcurrentToken = 1
        };
        _context.AnalysisByEventType.Add(db);
        return db;
    }

    private static void UpdateStoredAnalysis(AnalysisByEventType db, AnalysisByEventType incoming) {
        db.IsOn      = incoming.IsOn;
        db.Settings  = incoming.Settings;
        db.UpdatedAt = DateTime.UtcNow;
        db.ConcurrentToken++;
    }
}