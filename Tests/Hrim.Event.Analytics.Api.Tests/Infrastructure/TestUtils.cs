using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hrim.Event.Analytics.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

[ExcludeFromCodeCoverage]
public static class TestUtils
{
    public static readonly DateTimeOffset DayStart = new(year: 2022,
                                                         month: 01,
                                                         day: 01,
                                                         hour: 0,
                                                         minute: 0,
                                                         second: 0,
                                                         offset: TimeSpan.Zero);

    public static readonly DateTimeOffset DayEnd = new(year: 2022,
                                                       month: 01,
                                                       day: 01,
                                                       hour: 23,
                                                       minute: 59,
                                                       second: 59,
                                                       offset: TimeSpan.Zero);

    /// <summary>
    ///     Cleans up previous registrations of a type
    /// </summary>
    public static void CleanUpCurrentRegistrations(this IServiceCollection services, Type type) {
        var descriptors = services.Where(d => d.ServiceType == type)
                                  .ToList();
        descriptors.ForEach(x => services.Remove(item: x));
    }

    /// <summary>
    ///     Create string content from an instance
    /// </summary>
    public static StringContent PrepareJson<T>(T instance) {
        return new StringContent(JsonConvert.SerializeObject(value: instance, JsonSettingsFactory.Get()),
                                 encoding: Encoding.UTF8,
                                 mediaType: "application/json");
    }

    public static string GenerateString(int length) {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++) {
            sb.Append('a');
        }
        return sb.ToString();
    }
}