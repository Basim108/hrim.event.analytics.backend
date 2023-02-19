using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
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
[SuppressMessage("Usage",
    "xUnit1033:Test classes decorated with \'Xunit.IClassFixture<TFixture>\' or \'Xunit.ICollectionFixture<TFixture>\' should add a constructor argument of type TFixture")]
public abstract class BaseEventControllerValidationTests : BaseEntityControllerTests, IDisposable
{
    private readonly Guid _operatorId;
    private readonly IServiceScope _serviceScope;
    private readonly TestData _testData;
    protected readonly JsonSerializerSettings JsonSettings = JsonSettingsFactory.Get();

    protected BaseEventControllerValidationTests(WebAppFactory<Program> factory)
    {
        _serviceScope = factory.Services.CreateScope();
        var context = _serviceScope.ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        _testData = new TestData(context);
        var apiRequestAccessor = _serviceScope.ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        _operatorId = apiRequestAccessor.GetAuthorizedUserId();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing) _serviceScope.Dispose();
    }

    protected abstract BaseEvent GetBaseEventCreateRequest();

    protected abstract BaseEvent GetBaseEventUpdateRequest();

    [Theory]
    [InlineData("12a7e462-19d2-47cf-80e1-368be629dba7")]
    public async Task Create_Given_NonExistent_EventTypeId_Returns_BadRequest(string eventTypeId)
    {
        var createRequest = GetBaseEventCreateRequest();
        createRequest.EventTypeId = Guid.Parse(eventTypeId);

        var response = await Client!.PostAsync("", TestUtils.PrepareJson(createRequest));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(createRequest.EventTypeId).ToSnakeCase())
            .Should().BeTrue();
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Update_Given_Wrong_CreatedById_Returns_BadRequest(string createdById)
    {
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.CreatedById = Guid.Empty;
        var payload = JsonConvert.SerializeObject(updateRequest, JsonSettings)
            .Replace(Guid.Empty.ToString(), createdById);

        var response = await Client!.PutAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(updateRequest.CreatedById).ToSnakeCase())
            .Should().BeTrue();
    }

    [Theory]
    [InlineData("12a7e462-19d2-47cf-80e1-368be629dba7")]
    public async Task Update_Given_NonExistent_CreatedById_Returns_BadRequest(string createdById)
    {
        var eventType = _testData.Events.CreateEventType(_operatorId, $"Headache-{Guid.NewGuid()}");
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.EventTypeId = eventType.Id;
        updateRequest.CreatedById = Guid.Parse(createdById);

        var response = await Client!.PutAsync("", TestUtils.PrepareJson(updateRequest));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(updateRequest.CreatedById).ToSnakeCase())
            .Should().BeTrue();
    }

    [Theory]
    [InlineData("12a7e462-19d2-47cf-80e1-368be629dba7")]
    public async Task Update_Given_NonExistent_EventTypeId_Returns_BadRequest(string eventTypeId)
    {
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.EventTypeId = Guid.Parse(eventTypeId);

        var response = await Client!.PutAsync("", TestUtils.PrepareJson(updateRequest));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
            .ContainsKey(nameof(updateRequest.EventTypeId).ToSnakeCase())
            .Should().BeTrue();
    }
}