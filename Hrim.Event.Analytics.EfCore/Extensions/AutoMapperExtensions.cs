using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.EfCore.AutoMapper;
using Hrim.Event.Analytics.EfCore.DbEntities;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;

namespace Hrim.Event.Analytics.EfCore.Extensions;

public static class AutoMapperExtensions
{
    public static TEntity? ProjectFromDb<TEntity, TKey>(this IMapper mapper, HrimEntity<TKey>? existed)
        where TKey : struct
        where TEntity : HrimEntity<TKey> {
        return existed switch {
            DbDurationEvent   => mapper.Map<TEntity>(source: existed),
            DbOccurrenceEvent => mapper.Map<TEntity>(source: existed),
            DbEventType       => mapper.Map<TEntity>(source: existed),
            _                 => existed as TEntity
        };
    }
}