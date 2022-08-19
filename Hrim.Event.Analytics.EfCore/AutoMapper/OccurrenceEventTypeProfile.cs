using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.AutoMapper.Converters;
using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;

namespace Hrim.Event.Analytics.EfCore.AutoMapper; 

public class OccurrenceEventTypeProfile: Profile {
    public OccurrenceEventTypeProfile() {
        CreateMap<DbOccurrenceEventType, OccurrenceEventType>()
           .ForMember(business => business.OccurredAt,  x => x.MapFrom<OccurrenceOnValueResolver>())
           .ReverseMap()
           .ForMember(db => db.OccurredOn, x => x.MapFrom<OccurrenceOnValueResolver>());
    }
}