using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrimsoft.Core.Extensions;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.Events;

[ExcludeFromCodeCoverage]
public class DurationEventControllerValidationTests: BaseEventControllerValidationTests {
    public DurationEventControllerValidationTests(WebAppFactory<Program> factory) {
        Client = factory.GetClient("v1/event/duration/");
    }

    /// <summary> Correct duration event create request </summary>
    private readonly DurationEventCreateRequest _durationEventCreateRequest = new() {
        CreatedById = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        StartedAt   = new DateTimeOffset(2020, 09, 1, 15, 0, 0, TimeSpan.Zero),
        FinishedAt  = new DateTimeOffset(2020, 09, 1, 16, 0, 0, TimeSpan.Zero)
    };

    /// <summary> Correct duration event create request </summary>
    private readonly DurationEventUpdateRequest _durationEventUpdateRequest = new() {
        Id              = Guid.NewGuid(),
        CreatedById     = Guid.NewGuid(),
        EventTypeId     = Guid.NewGuid(),
        StartedAt       = new DateTimeOffset(2020, 09, 1, 15, 0, 0, TimeSpan.Zero),
        FinishedAt      = new DateTimeOffset(2020, 09, 1, 16, 0, 0, TimeSpan.Zero),
        CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
        ConcurrentToken = 1
    };

    protected override BaseEvent GetBaseEventCreateRequest() => _durationEventCreateRequest;

    protected override BaseEvent GetBaseEventUpdateRequest() => _durationEventUpdateRequest;

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Create_Given_Wrong_StartedAt_Returns_BadRequest(string startedAt) {
        _durationEventCreateRequest.StartedAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(_durationEventCreateRequest, JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString("O"), startedAt);

        var response = await Client!.PostAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.StartedAt).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Update_Given_Wrong_StartedAt_Returns_BadRequest(string startedAt) {
        _durationEventUpdateRequest.StartedAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(_durationEventUpdateRequest, JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString("O"), startedAt);

        var response = await Client!.PutAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.StartedAt).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Create_Given_Wrong_FinishedAt_Returns_BadRequest(string finishedAt) {
        _durationEventCreateRequest.FinishedAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(_durationEventCreateRequest, JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString("O"), finishedAt);

        var response = await Client!.PostAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.FinishedAt).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("0001-01-01T00:00:00.0000000+00:00")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Update_Given_Wrong_FinishedAt_Returns_BadRequest(string finishedAt) {
        _durationEventUpdateRequest.FinishedAt = DateTimeOffset.MinValue;
        var payload = JsonConvert.SerializeObject(_durationEventUpdateRequest, JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString("O"), finishedAt);

        var response = await Client!.PutAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.FinishedAt).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Theory]
    [InlineData("0001-01-03T00:00:00.0000000+00:00", "0001-01-02T00:00:00.0000000+00:00")]
    [InlineData("0001-01-03T00:00:00.0000000+00:00", "0001-01-03T00:00:00.0000000+00:00")]
    public async Task Create_Given_FinishedAt_BeforeOrSame_StartedAt_Returns_BadRequest(string startedAt, string finishedAt) {
        _durationEventCreateRequest.FinishedAt = DateTimeOffset.MinValue.AddDays(1).TruncateToSeconds();
        _durationEventCreateRequest.StartedAt  = DateTimeOffset.MinValue;
        var finishedTemplate = _durationEventCreateRequest.FinishedAt.Value.ToString("O");
        var payload = JsonConvert.SerializeObject(_durationEventCreateRequest, JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString("O"), startedAt)
                                 .Replace(finishedTemplate,                      finishedAt);

        var response = await Client!.PostAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.StartedAt).ToSnakeCase())
                      .Should().BeTrue();
    }
    
    [Theory]
    [InlineData("0001-01-03T00:00:00.0000000+00:00", "0001-01-02T00:00:00.0000000+00:00")]
    [InlineData("0001-01-03T00:00:00.0000000+00:00", "0001-01-03T00:00:00.0000000+00:00")]
    public async Task Update_Given_FinishedAt_BeforeOrSame_StartedAt_Returns_BadRequest(string startedAt, string finishedAt) {
        _durationEventUpdateRequest.FinishedAt = DateTimeOffset.MinValue.AddDays(1).TruncateToSeconds();
        _durationEventUpdateRequest.StartedAt  = DateTimeOffset.MinValue;
        var finishedTemplate = _durationEventUpdateRequest.FinishedAt.Value.ToString("O");
        var payload = JsonConvert.SerializeObject(_durationEventUpdateRequest, JsonSettings)
                                 .Replace(DateTimeOffset.MinValue.ToString("O"), startedAt)
                                 .Replace(finishedTemplate,                      finishedAt);

        var response = await Client!.PutAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(DurationEvent.StartedAt).ToSnakeCase())
                      .Should().BeTrue();
    }
}