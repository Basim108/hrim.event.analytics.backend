using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.Core.Extensions;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.Events;

[ExcludeFromCodeCoverage]
public class OccurrenceEventControllerValidationTests: BaseEventControllerValidationTests
{
    /// <summary> Correct duration event create request </summary>
    private readonly OccurrenceEventCreateRequest _occurrenceEventCreateRequest = new() {
        CreatedById = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        OccurredAt = new DateTimeOffset(year: 2020,
                                        month: 09,
                                        day: 1,
                                        hour: 15,
                                        minute: 0,
                                        second: 0,
                                        offset: TimeSpan.Zero)
    };

    /// <summary> Correct duration event create request </summary>
    private readonly OccurrenceEventUpdateRequest _occurrenceEventUpdateRequest = new() {
        Id          = Guid.NewGuid(),
        CreatedById = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        OccurredAt = new DateTimeOffset(year: 2020,
                                        month: 09,
                                        day: 1,
                                        hour: 15,
                                        minute: 0,
                                        second: 0,
                                        offset: TimeSpan.Zero),
        CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
        ConcurrentToken = 1
    };

    public OccurrenceEventControllerValidationTests(EventAnalyticsWebAppFactory<Program> factory): base(factory: factory) { Client = factory.GetClient(baseUrl: "v1/event/occurrence/"); }

    /// <summary> Correct create event request  </summary>
    protected override OccurrenceEventCreateRequest GetCreateRequest() {
        return new OccurrenceEventCreateRequest {
            OccurredAt = DateTimeOffset.Now
        };
    }

    /// <summary> Correct update event request  </summary>
    protected override OccurrenceEventUpdateRequest GetUpdateRequest() {
        return new OccurrenceEventUpdateRequest {
            Id              = Guid.NewGuid(),
            ConcurrentToken = 1,
            OccurredAt      = DateTimeOffset.Now
        };
    }

    protected override BaseEvent GetBaseEventCreateRequest() { return _occurrenceEventCreateRequest; }

    protected override BaseEvent GetBaseEventUpdateRequest() { return _occurrenceEventUpdateRequest; }

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Create_Given_Wrong_OccurredAt_Returns_BadRequest(string occurredAt) {
        _occurrenceEventCreateRequest.OccurredAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(value: _occurrenceEventCreateRequest, settings: JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString(format: "O"), newValue: occurredAt);

        var response = await Client!.PostAsync(requestUri: "", new StringContent(content: payload, encoding: Encoding.UTF8, mediaType: "application/json"));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(OccurrenceEvent.OccurredAt).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Update_Given_Wrong_OccurredAt_Returns_BadRequest(string occurredAt) {
        _occurrenceEventUpdateRequest.OccurredAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(value: _occurrenceEventUpdateRequest, settings: JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString(format: "O"), newValue: occurredAt);

        var response = await Client!.PutAsync(requestUri: "", new StringContent(content: payload, encoding: Encoding.UTF8, mediaType: "application/json"));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(OccurrenceEvent.OccurredAt).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }
}