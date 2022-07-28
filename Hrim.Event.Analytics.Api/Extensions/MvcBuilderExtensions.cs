using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hrim.Event.Analytics.Api.Extensions;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddHrimsoftJsonOptions(this IMvcBuilder builder)
        => builder.AddNewtonsoftJson(opt =>
        {
            opt.SerializerSettings.NullValueHandling    = NullValueHandling.Ignore;
            opt.SerializerSettings.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
            opt.SerializerSettings.TypeNameHandling     = TypeNameHandling.None;
            opt.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            opt.SerializerSettings.Converters = new List<JsonConverter>()
            {
                new StringEnumConverter
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        });
}