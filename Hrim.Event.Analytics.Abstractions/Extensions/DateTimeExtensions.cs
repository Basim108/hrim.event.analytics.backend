namespace Hrim.Event.Analytics.Abstractions.Extensions;

/// <summary>
///     Extends DateTime values
/// </summary>
public static class DateTimeExtensions
{
    /// <summary> Converts DateTime to DateOnly </summary>
    public static DateOnly ToDateOnly(this DateTime dateTime) { return new DateOnly(year: dateTime.Year, month: dateTime.Month, day: dateTime.Day); }
}