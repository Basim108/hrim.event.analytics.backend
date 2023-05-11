using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;

namespace Hrim.Event.Analytics.EfCore.Extensions;

public static class AutoMapperExtensions
{
    public static TEntity? ProjectFromDb<TEntity>(this IMapper mapper, HrimEntity? existed)
        where TEntity : class {
        return existed switch {
            DbDurationEvent   => mapper.Map<TEntity>(source: existed),
            DbOccurrenceEvent => mapper.Map<TEntity>(source: existed),
            _                 => existed as TEntity
        };
    }
}