using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestModels;
using Hrim.Event.Analytics.EfCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ControllerTests;

[ExcludeFromCodeCoverage]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class EntityControllerTests: IClassFixture<WebAppFactory<Program>>, IDisposable
{
    private readonly HttpClient             _client;
    private readonly JsonSerializerSettings _jsonSettings;
    private readonly IServiceScope          _serviceScope;
    private readonly TestData               _testData;
    private readonly IApiRequestAccessor    _requestAccessor;

    public EntityControllerTests(WebAppFactory<Program> factory) {
        _jsonSettings    = JsonSettingsFactory.Get();
        _client          = factory.GetClient("v1/entity/");
        _serviceScope    = factory.Services.CreateScope();
        _requestAccessor = _serviceScope.ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        var context = _serviceScope.ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        _testData = new TestData(context);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) _serviceScope.Dispose();
    }

    [Theory]
    [InlineData(EntityType.HrimUser)]
    [InlineData(EntityType.EventType)]
    [InlineData(EntityType.DurationEvent)]
    [InlineData(EntityType.OccurrenceEvent)]
    public async Task SoftDelete_Given_User_Should_Set_IsDeleted(EntityType entityType) {
        var operatorId = await _requestAccessor.GetInternalUserIdAsync(CancellationToken.None);
        HrimEntity entity = entityType switch {
            EntityType.HrimUser        => _testData.Users.EnsureUserExistence(operatorId, false),
            EntityType.EventType       => _testData.Events.CreateEventType(operatorId, $"name: {Guid.NewGuid()}", false),
            EntityType.DurationEvent   => _testData.Events.CreateDurationEvent(operatorId, isDeleted: false),
            EntityType.OccurrenceEvent => _testData.Events.CreateOccurrenceEvent(operatorId, isDeleted: false),
            _                          => throw new Exception($"Unsupported entity type: {entityType.ToString()}")
        };
        var url      = $"{entity.Id}?entity_type={entityType}";
        var response = await _client.DeleteAsync(url);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var respondedEntity = JsonConvert.DeserializeObject<TestEntity>(responseContent, _jsonSettings);
        respondedEntity.Should().NotBeNull();
        respondedEntity!.IsDeleted.Should().BeTrue();
    }

    [Theory]
    [InlineData(EntityType.HrimUser)]
    [InlineData(EntityType.EventType)]
    [InlineData(EntityType.DurationEvent)]
    [InlineData(EntityType.OccurrenceEvent)]
    public async Task Restore_Given_User_Should_Set_IsDeleted(EntityType entityType) {
        var operatorId = await _requestAccessor.GetInternalUserIdAsync(CancellationToken.None);
        HrimEntity entity = entityType switch {
            EntityType.HrimUser        => _testData.Users.EnsureUserExistence(operatorId, true),
            EntityType.EventType       => _testData.Events.CreateEventType(operatorId, $"name: {Guid.NewGuid()}", true),
            EntityType.DurationEvent   => _testData.Events.CreateDurationEvent(operatorId, isDeleted: true),
            EntityType.OccurrenceEvent => _testData.Events.CreateOccurrenceEvent(operatorId, isDeleted: true),
            _                          => throw new Exception($"Unsupported entity type: {entityType.ToString()}")
        };
        var url      = $"{entity.Id}?entity_type={entityType}";
        var response = await _client.PatchAsync(url, null);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var respondedEntity = JsonConvert.DeserializeObject<TestEntity>(responseContent, _jsonSettings);
        respondedEntity.Should().NotBeNull();
        respondedEntity!.IsDeleted.Should().NotBeTrue();
    }
}