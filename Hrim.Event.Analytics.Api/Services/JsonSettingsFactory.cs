using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Services;

/// <summary>
///     A single point to get a Json serializer settings
/// </summary>
public static class JsonSettingsFactory
{
    public static JsonSerializerSettings Get()
    {
        return new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            TypeNameHandling = TypeNameHandling.Objects,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter { NamingStrategy = new SnakeCaseNamingStrategy() }
            }
        };
    }
}