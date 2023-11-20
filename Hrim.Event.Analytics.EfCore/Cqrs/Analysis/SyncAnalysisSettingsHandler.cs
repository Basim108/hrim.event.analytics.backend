using Hrim.Event.Analytics.Abstractions.Cqrs.Analysis;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.EfCore.DbEntities.Analysis;
using Hrimsoft.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.EfCore.Cqrs.Analysis;

public class SyncAnalysisSettingsHandler: IRequestHandler<SyncAnalysisSettings, List<AnalysisConfigByEventType>?>
{
    private readonly IAnalysisSettingsFactory _settingsFactory;
    private readonly EventAnalyticDbContext   _context;

    public SyncAnalysisSettingsHandler(IAnalysisSettingsFactory settingsFactory,
                                       EventAnalyticDbContext   context) {
        _settingsFactory = settingsFactory;
        _context         = context;
    }

    public Task<List<AnalysisConfigByEventType>?> Handle(SyncAnalysisSettings request, CancellationToken cancellationToken) {
        if (request.EventTypeId == default)
            throw new ArgumentNullException(nameof(request), nameof(request.EventTypeId));

        return HandleAsync(request, cancellationToken);
    }

    private async Task<List<AnalysisConfigByEventType>?> HandleAsync(SyncAnalysisSettings request, CancellationToken cancellationToken) {
        var settings       = request.CurrentSettings;
        var missedSettings = _settingsFactory.GetMissedSettings(settings);
        if (missedSettings == null)
            return null;
        var availableMissedSettings = new List<AnalysisConfigByEventType>();
        var features = request.Features
                    ?? await _context.HrimFeatures
                                     .AsNoTracking()
                                     .Where(x => x.IsDeleted != true)
                                     .ToListAsync(cancellationToken);
        foreach (var missedItem in missedSettings) {
            var feature = features.First(x => x.Code == missedItem.AnalysisCode);
            if (!feature.IsOn || feature.IsDeleted == true)
                continue;
            var now = DateTime.UtcNow.TruncateToMicroseconds();
            var createdSettings = new DbAnalysisConfigByEventType {
                EventTypeId  = request.EventTypeId,
                AnalysisCode = missedItem.AnalysisCode,
                IsOn         = missedItem.IsOn,
                Settings = missedItem.Settings == null
                               ? null
                               : new Dictionary<string, string>(missedItem.Settings),
                CreatedAt       = now,
                UpdatedAt       = now,
                ConcurrentToken = 1
            };
            _context.AnalysisByEventType.Add(createdSettings);
            availableMissedSettings.Add(createdSettings);
        }
        if (request.IsSaveChanges)
            await _context.SaveChangesAsync(cancellationToken);
        return availableMissedSettings;
    }
}