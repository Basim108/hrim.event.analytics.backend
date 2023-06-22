using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.EfCore.ValueConverters;

public static class JsonDictionaryConverter
{
    public static ValueConverter<IDictionary<string, string>, string> Get() =>
        new(toProvider => JsonConvert.SerializeObject(toProvider),
            fromProvider => JsonConvert.DeserializeObject<IDictionary<string, string>>(fromProvider) ?? new Dictionary<string, string>());
    
    public static ValueConverter<IDictionary<string, string>?, string?> GetNullable() =>
        new(toProvider => toProvider == null 
                              ? null
                              : JsonConvert.SerializeObject(toProvider),
            fromProvider => string.IsNullOrWhiteSpace(fromProvider) 
                                ? null 
                                : JsonConvert.DeserializeObject<IDictionary<string, string>>(fromProvider));
}