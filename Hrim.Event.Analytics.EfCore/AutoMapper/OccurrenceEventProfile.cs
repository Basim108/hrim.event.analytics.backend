using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Events;
using Hrim.Event.Analytics.EfCore.AutoMapper.Converters;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;

namespace Hrim.Event.Analytics.EfCore.AutoMapper; 

public class OccurrenceEventTypeProfile: Profile {
    public OccurrenceEventTypeProfile() {
        CreateMap<DbOccurrenceEvent, OccurrenceEvent>()
           .ForMember(business => business.OccurredAt,  x => x.MapFrom<OccurrenceOnValueResolver>())
           .ReverseMap()
           .ForMember(db => db.OccurredOn, x => x.MapFrom<OccurrenceOnValueResolver>());
        
        CreateMap<OccurrenceEventCreateRequest, DbOccurrenceEvent>()
           .ForMember(db => db.OccurredOn, x => x.MapFrom<OccurrenceOnValueResolver>());
        CreateMap<OccurrenceEventUpdateRequest, DbOccurrenceEvent>()
           .ForMember(db => db.OccurredOn, x => x.MapFrom<OccurrenceOnValueResolver>());
    }
}