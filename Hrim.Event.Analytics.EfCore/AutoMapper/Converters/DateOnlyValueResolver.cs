using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrim.Event.Analytics.EfCore.DbEntities.EventTypes;

namespace Hrim.Event.Analytics.EfCore.AutoMapper.Converters;

public class DurationStartedOnValueResolver: IValueResolver<DbDurationEvent, DurationEvent, DateTimeOffset>,
                                     IValueResolver<DurationEvent, DbDurationEvent, DateOnly> {
    public DateTimeOffset Resolve(DbDurationEvent source, DurationEvent destination, DateTimeOffset destMember, ResolutionContext context)
        => new DateTimeOffset(source.StartedOn.Year, source.StartedOn.Month, source.StartedOn.Day,
                              source.StartedAt.Hour, source.StartedAt.Minute, source.StartedAt.Second,
                              source.StartedAt.Millisecond,
                              source.StartedAt.Offset);

    public DateOnly Resolve(DurationEvent source, DbDurationEvent destination, DateOnly destMember, ResolutionContext context)
        => new DateOnly(source.StartedAt.Year,
                        source.StartedAt.Month,
                        source.StartedAt.Day);
}

public class DurationFinishedOnValueResolver: IValueResolver<DbDurationEvent, DurationEvent, DateTimeOffset?>,
                                      IValueResolver<DurationEvent, DbDurationEvent, DateOnly?> {
    public DateTimeOffset? Resolve(DbDurationEvent source, DurationEvent destination, DateTimeOffset? destMember, ResolutionContext context)
        => source.FinishedOn == null || source.FinishedAt == null
               ? null
               : new DateTimeOffset(source.FinishedOn.Value.Year, source.FinishedOn.Value.Month, source.FinishedOn.Value.Day,
                                    source.FinishedAt.Value.Hour, source.FinishedAt.Value.Minute, source.FinishedAt.Value.Second,
                                    source.FinishedAt.Value.Millisecond,
                                    source.FinishedAt.Value.Offset);

    public DateOnly? Resolve(DurationEvent source, DbDurationEvent destination, DateOnly? destMember, ResolutionContext context)
        => source.FinishedAt == null
               ? null
               : new DateOnly(source.FinishedAt.Value.Year,
                              source.FinishedAt.Value.Month,
                              source.FinishedAt.Value.Day);
}

public class OccurrenceOnValueResolver: IValueResolver<DbOccurrenceEvent, OccurrenceEvent, DateTimeOffset>,
                                        IValueResolver<OccurrenceEvent, DbOccurrenceEvent, DateOnly> {
    public DateTimeOffset Resolve(DbOccurrenceEvent source, OccurrenceEvent destination, DateTimeOffset destMember, ResolutionContext context)
        => new DateTimeOffset(source.OccurredOn.Year, source.OccurredOn.Month, source.OccurredOn.Day,
                              source.OccurredAt.Hour, source.OccurredAt.Minute, source.OccurredAt.Second,
                              source.OccurredAt.Millisecond,
                              source.OccurredAt.Offset);

    public DateOnly Resolve(OccurrenceEvent source, DbOccurrenceEvent destination, DateOnly destMember, ResolutionContext context)
        => new DateOnly(source.OccurredAt.Year,
                        source.OccurredAt.Month,
                        source.OccurredAt.Day);
}