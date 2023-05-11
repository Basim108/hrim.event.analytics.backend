#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Extensions;

public static class DateOnlyExtensions
{
    /// <summary>
    ///     Combines date from DateOnly and time with timezone from DateTimeOffset
    /// </summary>
    public static DateTimeOffset CombineWithTime(this DateOnly date, DateTimeOffset time) {
        return new DateTimeOffset(year: date.Year,
                                  month: date.Month,
                                  day: date.Day,
                                  hour: time.Hour,
                                  minute: time.Minute,
                                  second: time.Second,
                                  millisecond: time.Millisecond,
                                  offset: time.Offset);
    }

    /// <summary>
    ///     Combines date from DateOnly and time with timezone from DateTimeOffset
    /// </summary>
    public static DateTimeOffset? CombineWithTime(this DateOnly? date, DateTimeOffset? time) {
        return date == null || time == null
                   ? null
                   : new DateTimeOffset(year: date.Value.Year,
                                        month: date.Value.Month,
                                        day: date.Value.Day,
                                        hour: time.Value.Hour,
                                        minute: time.Value.Minute,
                                        second: time.Value.Second,
                                        millisecond: time.Value.Millisecond,
                                        offset: time.Value.Offset);
    }
}