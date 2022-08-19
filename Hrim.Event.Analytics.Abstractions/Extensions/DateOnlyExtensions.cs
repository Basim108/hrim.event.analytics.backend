#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Extensions {
    public static class DateOnlyExtensions {
        /// <summary>
        /// Combines date from DateOnly and time with timezone from DateTimeOffset
        /// </summary>
        public static DateTimeOffset CombineWithTime(this DateOnly date, DateTimeOffset time)
            => new DateTimeOffset(date.Year, date.Month, date.Day,
                                  time.Hour, time.Minute, time.Second, time.Millisecond,
                                  time.Offset);

        /// <summary>
        /// Combines date from DateOnly and time with timezone from DateTimeOffset
        /// </summary>
        public static DateTimeOffset? CombineWithTime(this DateOnly? date, DateTimeOffset? time)
            => date == null || time == null
                   ? null
                   : new DateTimeOffset(date.Value.Year, date.Value.Month, date.Value.Day,
                                        time.Value.Hour, time.Value.Minute, time.Value.Second, time.Value.Millisecond,
                                        time.Value.Offset);
    }
}