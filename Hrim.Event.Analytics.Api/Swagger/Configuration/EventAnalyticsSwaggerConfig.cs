using Microsoft.OpenApi.Models;

#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Swagger.Configuration;

public static class SwaggerConfig
{
    public static OpenApiInfo MakeEventAnalytics()
    {
        return new OpenApiInfo
        {
            Version = "v1",
            Title = "Hrimsoft Event Analytics API",
            Description = "Api for crud operations around event infrastructure",
            Contact = new OpenApiContact
            {
                Name = "Hrim Event Analytics",
                Email = "support@hrimsoft.com"
            }
        };
    }
}