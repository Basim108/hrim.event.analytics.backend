using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;

namespace Hrim.Event.Analytics.EfCore.Extensions;

public static class AutoMapperExtensions {
    public static TEntity? ProjectFromDb<TEntity>(this IMapper mapper, Entity? existed)
        where TEntity : class
        => existed switch {
            DbDurationEventType   => mapper.Map<TEntity>(existed),
            DbOccurrenceEventType => mapper.Map<TEntity>(existed),
            _                     => existed as TEntity
        };
}