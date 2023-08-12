using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.EfCore;
using Hrimsoft.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

public class AnalysisByEventTypeData
{
    private readonly EventAnalyticDbContext _context;

    public AnalysisByEventTypeData(EventAnalyticDbContext context) { _context = context; }
    public AnalysisByEventType EnsureExistence(Guid eventTypeId, AnalysisByEventType template) {
        var now = DateTime.UtcNow.TruncateToMicroseconds();
        var analysis = new AnalysisByEventType {
            EventTypeId     = eventTypeId,
            AnalysisCode    = template.AnalysisCode,
            IsOn            = template.IsOn,
            Settings        = template.Settings,
            CreatedAt       = now,
            UpdatedAt       = now,
            ConcurrentToken = 1
        };
        _context.AnalysisByEventType.Add(analysis);
        _context.SaveChanges();
        return analysis;
    }

    public AnalysisByEventType EnsureExistence(Guid                         eventTypeId,
                                               string                       analysisCode,
                                               bool                         isOn,
                                               IDictionary<string, string>? settings) {
        var now = DateTime.UtcNow.TruncateToMicroseconds();
        var analysis = new AnalysisByEventType {
            EventTypeId     = eventTypeId,
            AnalysisCode    = analysisCode,
            IsOn            = isOn,
            Settings        = settings,
            CreatedAt       = now,
            UpdatedAt       = now,
            ConcurrentToken = 1
        };
        _context.AnalysisByEventType.Add(analysis);
        _context.SaveChanges();
        return analysis;
    }

    public Task<AnalysisByEventType?> GetAsync(Guid eventTypeId, string analysisCode)
        => _context.AnalysisByEventType.FirstOrDefaultAsync(x => x.EventTypeId == eventTypeId && x.AnalysisCode == analysisCode);
}