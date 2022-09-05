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
public class OccurrenceEventControllerValidationTests: BaseEventControllerValidationTests {
    public OccurrenceEventControllerValidationTests(WebAppFactory<Program> factory) {
        Client = factory.GetClient("v1/event/occurrence/");
    }

    /// <summary> Correct duration event create request </summary>
    private readonly OccurrenceEventCreateRequest _occurrenceEventCreateRequest = new() {
        CreatedById = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        OccurredAt  = new DateTimeOffset(2020, 09, 1, 15, 0, 0, TimeSpan.Zero)
    };

    /// <summary> Correct duration event create request </summary>
    private readonly OccurrenceEventUpdateRequest _occurrenceEventUpdateRequest = new() {
        Id              = Guid.NewGuid(),
        CreatedById     = Guid.NewGuid(),
        EventTypeId     = Guid.NewGuid(),
        OccurredAt      = new DateTimeOffset(2020, 09, 1, 15, 0, 0, TimeSpan.Zero),
        CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
        ConcurrentToken = 1
    };

    protected override BaseEvent GetBaseEventCreateRequest() => _occurrenceEventCreateRequest;

    protected override BaseEvent GetBaseEventUpdateRequest() => _occurrenceEventUpdateRequest;

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Create_Given_Wrong_OccurredAt_Returns_BadRequest(string occurredAt) {
        _occurrenceEventCreateRequest.OccurredAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(_occurrenceEventCreateRequest, JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString("O"), occurredAt);

        var response = await Client!.PostAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(OccurrenceEvent.OccurredAt).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Update_Given_Wrong_OccurredAt_Returns_BadRequest(string occurredAt) {
        _occurrenceEventUpdateRequest.OccurredAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(_occurrenceEventUpdateRequest, JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString("O"), occurredAt);

        var response = await Client!.PutAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(OccurrenceEvent.OccurredAt).ToSnakeCase())
                      .Should().BeTrue();
    }
}