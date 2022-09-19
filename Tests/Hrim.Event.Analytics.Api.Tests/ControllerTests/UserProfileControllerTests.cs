using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Users;
using Hrim.Event.Analytics.Api.Services;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.Tests.ControllerTests;

[ExcludeFromCodeCoverage]
public class UserProfileControllerTests: IClassFixture<WebAppFactory<Program>> {
    private readonly HttpClient             _client;
    private readonly JsonSerializerSettings _jsonSettings;

    public UserProfileControllerTests(WebAppFactory<Program> factory) {
        _jsonSettings = JsonSettingsFactory.Get();
        _client       = factory.GetClient("v1/user-profile/");
    }
    
    [Fact]
    public async Task GetMe_Should_Set_FullName() {
        var response = await _client.GetAsync("me");

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var profile         = JsonConvert.DeserializeObject<ViewHrimUser>(responseContent, _jsonSettings);
        profile.Should().NotBeNull();
        profile!.FullName.Should().Be(TestAuthHandler.NAME);
    }
    
    [Fact]
    public async Task GetMe_Should_Set_PictureUri() {
        var response = await _client.GetAsync("me");

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var profile         = JsonConvert.DeserializeObject<ViewHrimUser>(responseContent, _jsonSettings);
        profile.Should().NotBeNull();
        profile!.PictureUri.Should().Be(TestAuthHandler.PICTURE_URI);
    }
}