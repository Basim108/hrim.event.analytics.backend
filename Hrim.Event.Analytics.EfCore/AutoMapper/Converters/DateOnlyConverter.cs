using AutoMapper;

namespace Hrim.Event.Analytics.EfCore.AutoMapper.Converters; 

public class DateTimeOffsetConverter: IValueConverter<DateTimeOffset, DateOnly> {
    public DateOnly Convert(DateTimeOffset source, ResolutionContext context) 
        => new (source.Year, source.Month, source.Day);
}
public class NullableDateTimeOffsetConverter: IValueConverter<DateTimeOffset?, DateOnly?> {
    public DateOnly? Convert(DateTimeOffset? source, ResolutionContext context) 
        => source == null 
               ? null
               : new DateOnly(source.Value.Year, source.Value.Month, source.Value.Day);
}