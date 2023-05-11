using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hrim.Event.Analytics.Api.Extensions;

/// <summary> Mvc customizations </summary>
public static class MvcBuilderExtensions
{
    /// <summary> Set json serialization settings </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddHrimsoftJsonOptions(this IMvcBuilder builder) {
        return builder.AddNewtonsoftJson(opt => {
            opt.SerializerSettings.NullValueHandling    = NullValueHandling.Ignore;
            opt.SerializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
            opt.SerializerSettings.TypeNameHandling     = TypeNameHandling.None;
            opt.SerializerSettings.ContractResolver = new DefaultContractResolver {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            opt.SerializerSettings.Converters = new List<JsonConverter> {
                new StringEnumConverter {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        });
    }
}