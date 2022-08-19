using Microsoft.OpenApi.Models;
#pragma warning disable CS1591

namespace Hrim.Event.Analytics.Api.Swagger.Configuration; 

public static class SwaggerConfig
{
    public static OpenApiInfo MakeEventAnalytics()
    {
        return new()
        {
            Version        = "v1",
            Title          = "Hrimsoft Event Analytics API",
            Description    = "Api for crud operations around event infrastructure",
            TermsOfService = new Uri("https://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/98426/Concept"),
            Contact = new OpenApiContact
            {
                Name  = "Hrim Event Analytics",
                Email = "support@hrimsoft.com",
                Url   = new Uri("https://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/98426/Concept")
            }
        };
    }
}