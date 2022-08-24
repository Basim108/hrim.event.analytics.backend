using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.ViewModels.Events;
using Hrim.Event.Analytics.EfCore.AutoMapper.Converters;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;

namespace Hrim.Event.Analytics.EfCore.AutoMapper; 

public class DurationEventTypeProfile: Profile {
    public DurationEventTypeProfile() {
        CreateMap<DbDurationEvent, DurationEvent>()
           .ForMember(business => business.StartedAt,  x => x.MapFrom<DurationStartedOnValueResolver>())
           .ForMember(business => business.FinishedAt, x => x.MapFrom<DurationFinishedOnValueResolver>())
           .ReverseMap()
           .ForMember(db => db.StartedOn,  x => x.MapFrom<DurationStartedOnValueResolver>())
           .ForMember(db => db.FinishedOn, x => x.MapFrom<DurationFinishedOnValueResolver>());
        
        CreateMap<DurationEventCreateRequest, DbDurationEvent>()
           .ForMember(db => db.StartedOn,  x => x.MapFrom<DurationStartedOnValueResolver>())
           .ForMember(db => db.FinishedOn, x => x.MapFrom<DurationFinishedOnValueResolver>());
        
        CreateMap<DurationEventUpdateRequest, DbDurationEvent>()
           .ForMember(db => db.StartedOn,  x => x.MapFrom<DurationStartedOnValueResolver>())
           .ForMember(db => db.FinishedOn, x => x.MapFrom<DurationFinishedOnValueResolver>());
    }
}