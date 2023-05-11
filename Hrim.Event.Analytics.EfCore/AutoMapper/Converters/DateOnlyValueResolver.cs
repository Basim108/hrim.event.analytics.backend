using AutoMapper;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;

namespace Hrim.Event.Analytics.EfCore.AutoMapper.Converters;

public class DurationStartedOnValueResolver: IValueResolver<DbDurationEvent, DurationEvent, DateTimeOffset>,
                                             IValueResolver<DurationEvent, DbDurationEvent, DateOnly>
{
    public DateTimeOffset Resolve(DbDurationEvent   source,
                                  DurationEvent     destination,
                                  DateTimeOffset    destMember,
                                  ResolutionContext context) {
        return new DateTimeOffset(year: source.StartedOn.Year,
                                  month: source.StartedOn.Month,
                                  day: source.StartedOn.Day,
                                  hour: source.StartedAt.Hour,
                                  minute: source.StartedAt.Minute,
                                  second: source.StartedAt.Second,
                                  millisecond: source.StartedAt.Millisecond,
                                  offset: source.StartedAt.Offset);
    }

    public DateOnly Resolve(DurationEvent     source,
                            DbDurationEvent   destination,
                            DateOnly          destMember,
                            ResolutionContext context) {
        return new DateOnly(year: source.StartedAt.Year,
                            month: source.StartedAt.Month,
                            day: source.StartedAt.Day);
    }
}

public class DurationFinishedOnValueResolver: IValueResolver<DbDurationEvent, DurationEvent, DateTimeOffset?>,
                                              IValueResolver<DurationEvent, DbDurationEvent, DateOnly?>
{
    public DateTimeOffset? Resolve(DbDurationEvent   source,
                                   DurationEvent     destination,
                                   DateTimeOffset?   destMember,
                                   ResolutionContext context) {
        return source.FinishedOn == null || source.FinishedAt == null
                   ? null
                   : new DateTimeOffset(year: source.FinishedOn.Value.Year,
                                        month: source.FinishedOn.Value.Month,
                                        day: source.FinishedOn.Value.Day,
                                        hour: source.FinishedAt.Value.Hour,
                                        minute: source.FinishedAt.Value.Minute,
                                        second: source.FinishedAt.Value.Second,
                                        millisecond: source.FinishedAt.Value.Millisecond,
                                        offset: source.FinishedAt.Value.Offset);
    }

    public DateOnly? Resolve(DurationEvent     source,
                             DbDurationEvent   destination,
                             DateOnly?         destMember,
                             ResolutionContext context) {
        return source.FinishedAt == null
                   ? null
                   : new DateOnly(year: source.FinishedAt.Value.Year,
                                  month: source.FinishedAt.Value.Month,
                                  day: source.FinishedAt.Value.Day);
    }
}

public class OccurrenceOnValueResolver: IValueResolver<DbOccurrenceEvent, OccurrenceEvent, DateTimeOffset>,
                                        IValueResolver<OccurrenceEvent, DbOccurrenceEvent, DateOnly>
{
    public DateTimeOffset Resolve(DbOccurrenceEvent source,
                                  OccurrenceEvent   destination,
                                  DateTimeOffset    destMember,
                                  ResolutionContext context) {
        return new DateTimeOffset(year: source.OccurredOn.Year,
                                  month: source.OccurredOn.Month,
                                  day: source.OccurredOn.Day,
                                  hour: source.OccurredAt.Hour,
                                  minute: source.OccurredAt.Minute,
                                  second: source.OccurredAt.Second,
                                  millisecond: source.OccurredAt.Millisecond,
                                  offset: source.OccurredAt.Offset);
    }

    public DateOnly Resolve(OccurrenceEvent   source,
                            DbOccurrenceEvent destination,
                            DateOnly          destMember,
                            ResolutionContext context) {
        return new DateOnly(year: source.OccurredAt.Year,
                            month: source.OccurredAt.Month,
                            day: source.OccurredAt.Day);
    }
}