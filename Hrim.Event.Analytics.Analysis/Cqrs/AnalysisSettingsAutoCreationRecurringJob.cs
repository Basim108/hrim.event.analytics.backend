using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.EfCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hrim.Event.Analytics.Analysis.Cqrs;

public class AnalysisSettingsAutoCreationRecurringJobHandler: IRequestHandler<AnalysisSettingsAutoCreationRecurringJob>
{
    private readonly ILogger<AnalysisSettingsAutoCreationRecurringJobHandler> _logger;
    private readonly IMediator                                                _mediator;
    private readonly EventAnalyticDbContext                                   _context;

    public AnalysisSettingsAutoCreationRecurringJobHandler(ILogger<AnalysisSettingsAutoCreationRecurringJobHandler> logger,
                                                           IMediator                                                mediator,
                                                           EventAnalyticDbContext                                   context) {
        _logger   = logger;
        _mediator = mediator;
        _context  = context;
    }

    public async Task Handle(AnalysisSettingsAutoCreationRecurringJob request, CancellationToken cancellationToken) {
        var eventTypes = await _context.UserEventTypes
                                       .Include(x => x.AnalysisSettings)
                                       .Where(x => x.IsDeleted != true)
                                       .AsNoTracking()
                                       .ToListAsync(cancellationToken);
        var features = await _context.HrimFeatures
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
        foreach (var eventType in eventTypes) {
            var syncCommand    = new SyncAnalysisSettings(eventType.Id, eventType.AnalysisSettings, features, IsSaveChanges: false);
            var missedSettings = await _mediator.Send(syncCommand, cancellationToken);
            if (_logger.IsEnabled(LogLevel.Debug)) {
                var missedCodes = missedSettings == null
                                      ? "no missed settings found"
                                      : string.Join(", ", missedSettings.Select(x => x.AnalysisCode));
                _logger.LogDebug(EfCoreLogs.PROCESSED_EVENT_TYPE, eventType.Id, missedCodes);
            }
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}