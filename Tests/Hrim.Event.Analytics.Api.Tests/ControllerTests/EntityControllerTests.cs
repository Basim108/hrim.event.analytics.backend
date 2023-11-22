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
public class EntityControllerTests: IClassFixture<EventAnalyticsWebAppFactory<Program>>, IDisposable
{
    private readonly HttpClient             _client;
    private readonly JsonSerializerSettings _jsonSettings;
    private readonly IApiRequestAccessor    _requestAccessor;
    private readonly IServiceScope          _serviceScope;
    private readonly TestData               _testData;

    public EntityControllerTests(EventAnalyticsWebAppFactory<Program> factory) {
        _jsonSettings    = JsonSettingsFactory.Get();
        _client          = factory.GetClient(baseUrl: "v1/entity/");
        _serviceScope    = factory.Services.CreateScope();
        _requestAccessor = _serviceScope.ServiceProvider.GetRequiredService<IApiRequestAccessor>();
        var context = _serviceScope.ServiceProvider.GetRequiredService<EventAnalyticDbContext>();
        _testData = new TestData(context, MapperFactory.GetMapper());
    }

    public void Dispose() {
        Dispose(disposing: true);
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
        var operatorId = await _requestAccessor.GetInternalUserIdAsync(cancellation: CancellationToken.None);
        HrimEntity<long> entity = entityType switch {
            EntityType.HrimUser        => _testData.Users.EnsureUserExistence(id: operatorId),
            EntityType.EventType       => _testData.Events.CreateEventType(userId: operatorId, $"name: {Guid.NewGuid()}").Bl,
            EntityType.DurationEvent   => _testData.Events.CreateDurationEvent(userId: operatorId, isDeleted: false),
            EntityType.OccurrenceEvent => _testData.Events.CreateOccurrenceEvent(userId: operatorId, isDeleted: false),
            _                          => throw new Exception($"Unsupported entity type: {entityType.ToString()}")
        };
        var url      = $"{entity.Id}?entity_type={entityType}";
        var response = await _client.DeleteAsync(requestUri: url);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var respondedEntity = JsonConvert.DeserializeObject<TestEntity>(value: responseContent, settings: _jsonSettings);
        respondedEntity.Should().NotBeNull();
        respondedEntity!.IsDeleted.Should().BeTrue();
    }

    [Theory]
    [InlineData(EntityType.HrimUser)]
    [InlineData(EntityType.EventType)]
    [InlineData(EntityType.DurationEvent)]
    [InlineData(EntityType.OccurrenceEvent)]
    public async Task Restore_Given_User_Should_Set_IsDeleted(EntityType entityType) {
        var operatorId = await _requestAccessor.GetInternalUserIdAsync(cancellation: CancellationToken.None);
        HrimEntity<long> entity = entityType switch {
            EntityType.HrimUser        => _testData.Users.EnsureUserExistence(id: operatorId, isDeleted: true),
            EntityType.EventType       => _testData.Events.CreateEventType(userId: operatorId, $"name: {Guid.NewGuid()}", isDeleted: true).Bl,
            EntityType.DurationEvent   => _testData.Events.CreateDurationEvent(userId: operatorId, isDeleted: true),
            EntityType.OccurrenceEvent => _testData.Events.CreateOccurrenceEvent(userId: operatorId, isDeleted: true),
            _                          => throw new Exception($"Unsupported entity type: {entityType.ToString()}")
        };
        var url      = $"{entity.Id}?entity_type={entityType}";
        var response = await _client.PatchAsync(requestUri: url, content: null);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var respondedEntity = JsonConvert.DeserializeObject<TestEntity>(value: responseContent, settings: _jsonSettings);
        respondedEntity.Should().NotBeNull();
        respondedEntity!.IsDeleted.Should().NotBeTrue();
    }
}