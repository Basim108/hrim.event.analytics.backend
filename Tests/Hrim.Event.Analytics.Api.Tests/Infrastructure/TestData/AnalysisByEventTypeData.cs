using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.EfCore;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

public class AnalysisByEventTypeData
{
    private readonly EventAnalyticDbContext _context;

    public AnalysisByEventTypeData(EventAnalyticDbContext context) { _context = context; }

    public AnalysisByEventType EnsureExistence(Guid                         eventTypeId,
                                               string                       analysisCode,
                                               bool                         isOn,
                                               IDictionary<string, string>? settings) {
        var analysis = new AnalysisByEventType {
            EventTypeId     = eventTypeId,
            AnalysisCode    = analysisCode,
            IsOn            = isOn,
            Settings        = settings,
            CreatedAt       = DateTime.UtcNow,
            ConcurrentToken = 1
        };
        _context.AnalysisByEventType.Add(analysis);
        _context.SaveChanges();
        return analysis;
    }
}