using System.Net;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities.EventTypes;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.V1.Models;
using Hrim.Event.Analytics.EfCore;
using Hrimsoft.Core.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.Controllers.EventType;

public class EventTypeControllerIntegrationTests: IClassFixture<WebAppFactory<Program>> {
    private readonly EventAnalyticDbContext _context;
    private readonly HttpClient             _client;
    private readonly CreateEventTypeRequest _createRequest;
    private readonly JsonSerializerSettings _jsonSettings;
    private readonly Guid                   _userId = Guid.Parse("4e8712b8-1bdb-4bad-9047-9fc90251973e");

    public EventTypeControllerIntegrationTests(WebAppFactory<Program> factory) {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions() {
            BaseAddress = new Uri("/v1/event-type/")
        });
        _context      = factory.Services.CreateScope().ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        _jsonSettings = JsonSettingsFactory.Get();
        _createRequest = new() {
            Name        = "Headache",
            Color       = "#ff0000",
            Description = "times when I had a headache",
            IsPublic    = true
        };
    }

    private async Task CleanUpAsync() {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
        await TestUtils.CreateUserAsync(_userId, _context);
    }

    private async Task<List<Guid>> CreateManyEventTypes(int count, Guid userId, bool isDeleted = false) {
        var result = new List<Guid>(count);
        for (int i = 0; i < count; i++) {
            var entity = new UserEventType {
                Name            = $"event type {i}",
                Color           = "#f0c",
                IsPublic        = true,
                CreatedById     = userId,
                CreatedAt       = DateTime.UtcNow.TruncateToMicroseconds(),
                ConcurrentToken = 1
            };
            if (isDeleted)
                entity.IsDeleted = true;
            _context.UserEventTypes.Add(entity);
            await _context.SaveChangesAsync();
            result.Add(entity.Id);
        }
        return result;
    }

    [Fact]
    public async Task Create_EventType() {
        await CleanUpAsync();
        var beforeSend = DateTime.UtcNow;
        var response   = await _client.PostAsync("", TestUtils.PrepareJson(_createRequest));
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var resultEventType = JsonConvert.DeserializeObject<UserEventType>(responseContent, _jsonSettings);
        resultEventType.Should().NotBeNull();
        resultEventType!.Id.Should().NotBeEmpty();
        resultEventType.CreatedById.Should().Be(_userId);
        resultEventType.CreatedBy.Should().BeNull();
        resultEventType.CreatedAt.Should().BeAfter(beforeSend);
        resultEventType.UpdatedAt.Should().BeNull();
        resultEventType.IsDeleted.Should().BeNull();
        resultEventType.ConcurrentToken.Should().Be(1);

        resultEventType.Name.Should().Be(_createRequest.Name);
        resultEventType.Color.Should().Be(_createRequest.Color);
        resultEventType.Description.Should().Be(_createRequest.Description);
        resultEventType.IsPublic.Should().Be(_createRequest.IsPublic);
    }

    [Fact]
    public async Task Create_EventType_With_Same_Name() {
        await CleanUpAsync();
        await CreateManyEventTypes(1, _userId);
        _createRequest.Name = $"event type {0}";

        var response = await _client.PostAsync("", TestUtils.PrepareJson(_createRequest));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_Already_Deleted_Entity() {
        await CleanUpAsync();
        await CreateManyEventTypes(1, _userId, isDeleted: true);
        _createRequest.Name = $"event type {0}";

        var response = await _client.PostAsync("", TestUtils.PrepareJson(_createRequest));

        response.StatusCode.Should().Be(HttpStatusCode.Gone);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();
    }

    [Fact]
    public async Task Update_EventType() {
        await CleanUpAsync();
        var createResponse = await _client.PostAsync("", TestUtils.PrepareJson(_createRequest));
        createResponse.EnsureSuccessStatusCode();
        var createdContent   = await createResponse.Content.ReadAsStringAsync();
        var createdEventType = JsonConvert.DeserializeObject<UserEventType>(createdContent, _jsonSettings);

        var beforeSend = DateTime.UtcNow;
        createdEventType!.Name = "Updated";
        var response = await _client.PutAsync("", TestUtils.PrepareJson(createdEventType));
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var resultEventType = JsonConvert.DeserializeObject<UserEventType>(responseContent, _jsonSettings);
        resultEventType.Should().NotBeNull();
        resultEventType!.Id.Should().Be(createdEventType.Id);
        resultEventType.CreatedById.Should().NotBeEmpty();
        resultEventType.CreatedBy.Should().BeNull();
        resultEventType.CreatedAt.Should().Be(createdEventType.CreatedAt);
        resultEventType.UpdatedAt.Should().BeAfter(beforeSend);
        resultEventType.IsDeleted.Should().BeNull();
        resultEventType.ConcurrentToken.Should().Be(2);

        resultEventType.Name.Should().Be("Updated");
        resultEventType.Color.Should().Be(_createRequest.Color);
        resultEventType.Description.Should().Be(_createRequest.Description);
        resultEventType.IsPublic.Should().Be(_createRequest.IsPublic);
    }
    
    [Fact]
    public async Task Update_NotFound_EventType() {
        await CleanUpAsync();
        _createRequest.Id = Guid.NewGuid();
        _createRequest.ConcurrentToken = 1;
        
        var response = await _client.PutAsync("", TestUtils.PrepareJson(_createRequest));
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Update_With_Wrong_ConcurrentToken() {
        await CleanUpAsync();
        var createResponse = await _client.PostAsync("", TestUtils.PrepareJson(_createRequest));
        createResponse.EnsureSuccessStatusCode();
        var createdContent   = await createResponse.Content.ReadAsStringAsync();
        var createdEventType = JsonConvert.DeserializeObject<UserEventType>(createdContent, _jsonSettings);
        createdEventType!.Name           = "Updated";
        createdEventType.ConcurrentToken = 5;

        var response = await _client.PutAsync("", TestUtils.PrepareJson(createdEventType));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();
    }

    [Fact]
    public async Task Update_Already_Deleted_Entity() {
        await CleanUpAsync();
        var entityId       = await CreateManyEventTypes(1, _userId, isDeleted: true);
        var entityToUpdate = await _context.UserEventTypes.FirstAsync(x => x.Id == entityId[0]);
        entityToUpdate.CreatedBy = null;
        entityToUpdate.Name      = "Updated";

        var response = await _client.PutAsync("", TestUtils.PrepareJson(entityToUpdate));

        response.StatusCode.Should().Be(HttpStatusCode.Gone);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();
    }

    [Fact]
    public async Task Update_Given_Not_My_EventType_Forbid() {
        await CleanUpAsync();
        var anotherUserId    = Guid.NewGuid();
        var anotherEntityIds = await CreateManyEventTypes(1, anotherUserId);
        _createRequest.Id              = anotherEntityIds[0];
        _createRequest.ConcurrentToken = 1;

        var response = await _client.PutAsync("", TestUtils.PrepareJson(_createRequest));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be(ApiLogs.FORBID_AS_NOT_ENTITY_OWNER);
    }

    [Fact]
    public async Task GetAll_Returns_Owned_EventTypes() {
        await CleanUpAsync();
        var anotherUserId = Guid.NewGuid();
        await TestUtils.CreateUserAsync(anotherUserId, _context);
        var myEventIds      = await CreateManyEventTypes(4, _userId);
        var anotherEventIds = await CreateManyEventTypes(1, anotherUserId);

        var response = await _client.GetAsync("");
        response.EnsureSuccessStatusCode();
        var content    = await response.Content.ReadAsStringAsync();
        var resultList = JsonConvert.DeserializeObject<List<UserEventType>>(content, _jsonSettings);
        resultList.Should().NotBeEmpty();
        resultList!.Count.Should().Be(4);
        resultList.All(x => myEventIds.Contains(x.Id)).Should().BeTrue();
        resultList.All(x => !anotherEventIds.Contains(x.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task GetById_Returns_Owned_EventTypes() {
        await CleanUpAsync();
        var anotherUserId = Guid.NewGuid();
        await TestUtils.CreateUserAsync(anotherUserId, _context);
        var myEventIds = await CreateManyEventTypes(4, _userId);
        await CreateManyEventTypes(1, anotherUserId);

        var targetId = myEventIds[1];
        var response = await _client.GetAsync(targetId.ToString());
        response.EnsureSuccessStatusCode();
        var content      = await response.Content.ReadAsStringAsync();
        var resultEntity = JsonConvert.DeserializeObject<UserEventType>(content, _jsonSettings);
        resultEntity.Should().NotBeNull();
        resultEntity!.Id.Should().Be(targetId);
    }

    [Fact]
    public async Task GetById_Given_AnotherId_Returns_Forbidden() {
        await CleanUpAsync();
        var anotherUserId = Guid.NewGuid();
        await TestUtils.CreateUserAsync(anotherUserId, _context);
        await CreateManyEventTypes(4, _userId);
        var anotherEventIds = await CreateManyEventTypes(1, anotherUserId);

        var targetId = anotherEventIds[0];
        var response = await _client.GetAsync(targetId.ToString());
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be(ApiLogs.FORBID_AS_NOT_ENTITY_OWNER);
    }
}