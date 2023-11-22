using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Abstractions.Services;
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
    private readonly long          _operatorId;
    private readonly IServiceScope _serviceScope;
    private readonly TestData      _testData;

    protected BaseEventControllerValidationTests(EventAnalyticsWebAppFactory<Program> factory) {
        _serviceScope = factory.Services.CreateScope();
        var context = _serviceScope.ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        _testData = new TestData(context, MapperFactory.GetMapper());
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
    [InlineData(2)]
    public async Task Create_Given_NonExistent_EventTypeId_Returns_BadRequest(long eventTypeId) {
        var createRequest = GetBaseEventCreateRequest();
        createRequest.EventTypeId = eventTypeId;

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
    [InlineData("0")]
    [InlineData("-2")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Update_Given_Wrong_CreatedById_Returns_BadRequest(string createdById) {
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.CreatedById = default;
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
    [InlineData(12345678)]
    public async Task Update_Given_NonExistent_CreatedById_Returns_BadRequest(long createdById) {
        var eventType     = _testData.Events.CreateEventType(userId: _operatorId, $"Headache-{Guid.NewGuid()}").Bl;
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.EventTypeId = eventType.Id;
        updateRequest.CreatedById = createdById;

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
    [InlineData(12345678)]
    public async Task Update_Given_NonExistent_EventTypeId_Returns_BadRequest(long eventTypeId) {
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.EventTypeId = eventTypeId;

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