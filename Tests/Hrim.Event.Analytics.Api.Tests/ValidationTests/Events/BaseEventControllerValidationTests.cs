using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Hrim.Event.Analytics.EfCore;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.Events;

[ExcludeFromCodeCoverage]
[SuppressMessage(category: "Usage",
                 checkId:
                 "xUnit1033:Test classes decorated with \'Xunit.IClassFixture<TFixture>\' or \'Xunit.ICollectionFixture<TFixture>\' should add a constructor argument of type TFixture")]
public abstract class BaseEventControllerValidationTests: BaseEntityControllerTests, IDisposable
{
    private readonly   Guid                   _operatorId;
    private readonly   IServiceScope          _serviceScope;
    private readonly   TestData               _testData;
    protected readonly JsonSerializerSettings JsonSettings = JsonSettingsFactory.Get();

    protected BaseEventControllerValidationTests(EventAnalyticsWebAppFactory<Program> factory) {
        _serviceScope = factory.Services.CreateScope();
        var context = _serviceScope.ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        _testData = new TestData(context: context);
        var apiRequestAccessor = _serviceScope.ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        _operatorId = apiRequestAccessor.GetInternalUserIdAsync(cancellation: CancellationToken.None).Result;
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) _serviceScope.Dispose();
    }

    protected abstract BaseEvent GetBaseEventCreateRequest();

    protected abstract BaseEvent GetBaseEventUpdateRequest();

    [Theory]
    [InlineData("12a7e462-19d2-47cf-80e1-368be629dba7")]
    public async Task Create_Given_NonExistent_EventTypeId_Returns_BadRequest(string eventTypeId) {
        var createRequest = GetBaseEventCreateRequest();
        createRequest.EventTypeId = Guid.Parse(input: eventTypeId);

        var response = await Client!.PostAsync(requestUri: "", TestUtils.PrepareJson(instance: createRequest));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.EventTypeId).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Update_Given_Wrong_CreatedById_Returns_BadRequest(string createdById) {
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.CreatedById = Guid.Empty;
        var payload = JsonConvert.SerializeObject(value: updateRequest, settings: JsonSettings)
                                 .Replace(Guid.Empty.ToString(), newValue: createdById);

        var response = await Client!.PutAsync(requestUri: "", new StringContent(content: payload, encoding: Encoding.UTF8, mediaType: "application/json"));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.CreatedById).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Theory]
    [InlineData("12a7e462-19d2-47cf-80e1-368be629dba7")]
    public async Task Update_Given_NonExistent_CreatedById_Returns_BadRequest(string createdById) {
        var eventType     = _testData.Events.CreateEventType(userId: _operatorId, $"Headache-{Guid.NewGuid()}");
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.EventTypeId = eventType.Id;
        updateRequest.CreatedById = Guid.Parse(input: createdById);

        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(instance: updateRequest));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.CreatedById).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }

    [Theory]
    [InlineData("12a7e462-19d2-47cf-80e1-368be629dba7")]
    public async Task Update_Given_NonExistent_EventTypeId_Returns_BadRequest(string eventTypeId) {
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.EventTypeId = Guid.Parse(input: eventTypeId);

        var response = await Client!.PutAsync(requestUri: "", TestUtils.PrepareJson(instance: updateRequest));

        response.StatusCode.Should().Be(expected: HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(value: responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.EventTypeId).ToSnakeCase())
                      .Should()
                      .BeTrue();
    }
}