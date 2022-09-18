using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.EfCore;
using Hrim.Event.Analytics.EfCore.DbEntities.Events;
using Hrimsoft.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

[ExcludeFromCodeCoverage]
public static class TestUtils {
    
    public static DateTimeOffset DayStart = new DateTimeOffset(2022, 01, 01, 0,  0,  0,  TimeSpan.Zero);
    public static DateTimeOffset DayEnd   = new DateTimeOffset(2022, 01, 01, 23, 59, 59, TimeSpan.Zero);
    
    /// <summary>
    /// Cleans up previous registrations of a type
    /// </summary>
    public static void CleanUpCurrentRegistrations(this IServiceCollection services, Type type) {
        var descriptors = services.Where(d => d.ServiceType == type)
                                  .ToList();
        descriptors.ForEach(x => services.Remove(x));
    }

    /// <summary>
    /// Create string content from an instance
    /// </summary>
    public static StringContent PrepareJson<T>(T instance) {
        return new StringContent(JsonConvert.SerializeObject(instance, JsonSettingsFactory.Get()),
                                 Encoding.UTF8,
                                 "application/json");
    }
}