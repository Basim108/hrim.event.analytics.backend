using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.AutoMapper.Converters;
using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;

namespace Hrim.Event.Analytics.EfCore.AutoMapper; 

public class DurationEventTypeProfile: Profile {
    public DurationEventTypeProfile() {
        CreateMap<DbDurationEventType, DurationEventType>()
           .ForMember(business => business.StartedAt,  x => x.MapFrom<StartedOnValueResolver>())
           .ForMember(business => business.FinishedAt, x => x.MapFrom<FinishedOnValueResolver>())
           .ReverseMap()
           .ForMember(db => db.FinishedOn, x => x.MapFrom<FinishedOnValueResolver>());
    }
}