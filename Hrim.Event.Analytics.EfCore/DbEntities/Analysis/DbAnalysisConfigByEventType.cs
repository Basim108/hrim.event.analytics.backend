using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.EfCore.DbEntities.Analysis;

public class DbAnalysisConfigByEventType: AnalysisConfigByEventType
{
    [JsonIgnore]
    public DbEventType EventType { get; set; }
}