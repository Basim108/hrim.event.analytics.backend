using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;

namespace Hrim.Event.Analytics.EfCore.AutoMapper.Converters;

public class StartedOnValueResolver: IValueResolver<DbDurationEventType, DurationEventType, DateTimeOffset> {
    public DateTimeOffset Resolve(DbDurationEventType source, DurationEventType destination, DateTimeOffset destMember, ResolutionContext context)
        => new DateTimeOffset(source.StartedOn.Year, source.StartedOn.Month, source.StartedOn.Day,
                              source.StartedAt.Hour, source.StartedAt.Minute, source.StartedAt.Second,
                              source.StartedAt.Millisecond,
                              source.StartedAt.Offset);
}

public class FinishedOnValueResolver: IValueResolver<DbDurationEventType, DurationEventType, DateTimeOffset?>,
                                      IValueResolver<DurationEventType, DbDurationEventType, DateOnly?> {
    public DateTimeOffset? Resolve(DbDurationEventType source, DurationEventType destination, DateTimeOffset? destMember, ResolutionContext context)
        => source.FinishedOn == null || source.FinishedAt == null
               ? null
               : new DateTimeOffset(source.FinishedOn.Value.Year, source.FinishedOn.Value.Month, source.FinishedOn.Value.Day,
                                    source.FinishedAt.Value.Hour, source.FinishedAt.Value.Minute, source.FinishedAt.Value.Second,
                                    source.FinishedAt.Value.Millisecond,
                                    source.FinishedAt.Value.Offset);

    public DateOnly? Resolve(DurationEventType source, DbDurationEventType destination, DateOnly? destMember, ResolutionContext context)
        => source.FinishedAt == null
               ? null
               : new DateOnly(source.FinishedAt.Value.Year,
                              source.FinishedAt.Value.Month,
                              source.FinishedAt.Value.Day);
}

public class OccurrenceOnValueResolver: IValueResolver<DbOccurrenceEventType, OccurrenceEventType, DateTimeOffset>,
                                        IValueResolver<OccurrenceEventType, DbOccurrenceEventType, DateOnly> {
    public DateTimeOffset Resolve(DbOccurrenceEventType source, OccurrenceEventType destination, DateTimeOffset destMember, ResolutionContext context)
        => new DateTimeOffset(source.OccurredOn.Year, source.OccurredOn.Month, source.OccurredOn.Day,
                              source.OccurredAt.Hour, source.OccurredAt.Minute, source.OccurredAt.Second,
                              source.OccurredAt.Millisecond,
                              source.OccurredAt.Offset);

    public DateOnly Resolve(OccurrenceEventType source, DbOccurrenceEventType destination, DateOnly destMember, ResolutionContext context)
        => new DateOnly(source.OccurredAt.Year,
                        source.OccurredAt.Month,
                        source.OccurredAt.Day);
}