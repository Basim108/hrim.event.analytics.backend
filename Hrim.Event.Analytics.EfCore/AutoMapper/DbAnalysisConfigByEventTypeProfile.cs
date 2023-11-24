using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrim.Event.Analytics.EfCore.DbEntities.Analysis;

namespace Hrim.Event.Analytics.EfCore.AutoMapper;

public class DbAnalysisConfigByEventTypeProfile: Profile
{
    public DbAnalysisConfigByEventTypeProfile() {
        CreateMap<DbAnalysisConfigByEventType, AnalysisConfigByEventType>()
           .ReverseMap();
    }
}