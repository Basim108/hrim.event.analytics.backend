using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.Events;
using Hrim.Event.Analytics.Api.Services;
using Hrimsoft.StringCases;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ValidationTests.Events;

[ExcludeFromCodeCoverage]
[SuppressMessage("Usage", "xUnit1033:Test classes decorated with \'Xunit.IClassFixture<TFixture>\' or \'Xunit.ICollectionFixture<TFixture>\' should add a constructor argument of type TFixture")]
public abstract class BaseEventControllerValidationTests: BaseEntityControllerTests {
    protected readonly JsonSerializerSettings JsonSettings = JsonSettingsFactory.Get();
    
    protected abstract BaseEvent GetBaseEventCreateRequest();

    protected abstract BaseEvent GetBaseEventUpdateRequest();

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Create_Given_Wrong_CreatedById_Returns_BadRequest(string createdById) {
        var createRequest = GetBaseEventCreateRequest();
        createRequest.CreatedById = Guid.Empty;
        var payload = JsonConvert.SerializeObject(createRequest, JsonSettings)
                                 .Replace(Guid.Empty.ToString(), createdById);
        
        var response = await Client!.PostAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(createRequest.CreatedById).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Update_Given_Wrong_CreatedById_Returns_BadRequest(string createdById) {
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.CreatedById = Guid.Empty;
        var payload = JsonConvert.SerializeObject(updateRequest, JsonSettings)
                                 .Replace(Guid.Empty.ToString(), createdById);
        
        var response = await Client!.PutAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.CreatedById).ToSnakeCase())
                      .Should().BeTrue();
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("352246af-9681")]
    [InlineData("352246af-9681-4aae-9c2c-6faddcb2e552-352246af-9681-4aae-9c2c-6faddcb2e552")]
    public async Task Create_Given_Wrong_EventTypeId_Returns_BadRequest(string eventTypeId) {
        var createRequest = GetBaseEventCreateRequest();
        createRequest.EventTypeId = Guid.Empty;
        var payload = JsonConvert.SerializeObject(createRequest, JsonSettings)
                                 .Replace(Guid.Empty.ToString(), eventTypeId);
        
        var response = await Client!.PostAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
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
    public async Task Update_Given_Wrong_EventTypeId_Returns_BadRequest(string eventTypeId) {
        var updateRequest = GetBaseEventUpdateRequest();
        updateRequest.EventTypeId = Guid.Empty;
        var payload = JsonConvert.SerializeObject(updateRequest, JsonSettings)
                                 .Replace(Guid.Empty.ToString(), eventTypeId);
        
        var response = await Client!.PutAsync("", new StringContent(payload, Encoding.UTF8, "application/json"));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails  = JsonConvert.DeserializeObject<ValidationProblemDetails>(responseContent);
        problemDetails.Should().NotBeNull();
        problemDetails!.Errors.Should().NotBeEmpty();
        problemDetails.Errors
                      .ContainsKey(nameof(updateRequest.EventTypeId).ToSnakeCase())
                      .Should().BeTrue();
    }
}