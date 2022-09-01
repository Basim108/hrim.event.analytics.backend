using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.EfCore;
using Hrimsoft.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure; 

[ExcludeFromCodeCoverage]
public static class TestUtils {
    /// <summary>
    /// Cleans up previous registrations of a type
    /// </summary>
    public static void CleanUpCurrentRegistrations(IServiceCollection services, Type type) {
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
    
    public static async Task CreateUserAsync(Guid id, EventAnalyticDbContext context) {
        var user = new HrimUser {
            Id              = id,
            CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
            ConcurrentToken = 1
        };
        context.HrimUsers.Add(user);
        await context.SaveChangesAsync();
    }
}