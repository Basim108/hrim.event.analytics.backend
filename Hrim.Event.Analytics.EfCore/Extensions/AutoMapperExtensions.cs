using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;

namespace Hrim.Event.Analytics.EfCore.Extensions;

public static class AutoMapperExtensions {
    public static TEntity? ProjectFromDb<TEntity>(this IMapper mapper, HrimEntity? existed)
        where TEntity : class
        => existed switch {
            DbDurationEvent   => mapper.Map<TEntity>(existed),
            DbOccurrenceEvent => mapper.Map<TEntity>(existed),
            _                 => existed as TEntity
        };
}