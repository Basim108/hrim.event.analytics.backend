using AutoMapper;

namespace Hrim.Event.Analytics.EfCore.AutoMapper.Converters;

public class DateTimeOffsetConverter: IValueConverter<DateTimeOffset, DateOnly>
{
    public DateOnly Convert(DateTimeOffset source, ResolutionContext context) { return new DateOnly(year: source.Year, month: source.Month, day: source.Day); }
}

public class NullableDateTimeOffsetConverter: IValueConverter<DateTimeOffset?, DateOnly?>
{
    public DateOnly? Convert(DateTimeOffset? source, ResolutionContext context) {
        return source == null
                   ? null
                   : new DateOnly(year: source.Value.Year, month: source.Value.Month, day: source.Value.Day);
    }
}