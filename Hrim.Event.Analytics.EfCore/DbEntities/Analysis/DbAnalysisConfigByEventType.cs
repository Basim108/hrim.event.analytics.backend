using Hrim.Event.Analytics.Abstractions.Entities.Analysis;

namespace Hrim.Event.Analytics.EfCore.DbEntities.Analysis;

public class DbAnalysisConfigByEventType: AnalysisConfigByEventType
{
    public DbEventType EventType { get; set; }
}