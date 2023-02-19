using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Users;

[ExcludeFromCodeCoverage]
public class ExternalUserProfileBuildTests : BaseCqrsTests
{
    private readonly IDictionary<string, string> _claims;

    public ExternalUserProfileBuildTests()
    {
        _claims = new Dictionary<string, string>
        {
            { ClaimTypes.NameIdentifier, UsersData.EXTERNAL_ID },
            { ClaimTypes.Email, UsersData.EMAIL },
            { ClaimTypes.Name, UsersData.FULL_NAME },
            { ClaimTypes.GivenName, UsersData.FIRST_NAME },
            { ClaimTypes.Surname, UsersData.LAST_NAME }
        };
    }

    [Fact]
    public async Task Facebook_Given_NameIdentifier_Claim_Builds_On_ExternalId()
    {
        var command = new ExternalUserProfileBuild(Guid.NewGuid(), _claims, ExternalIdp.Facebook);
        var profile = await Mediator.Send(command);
        profile.Should().NotBeNull();
        profile.ExternalUserId.Should().Be(UsersData.EXTERNAL_ID);
    }

    [Fact]
    public async Task Google_Given_NameIdentifier_Claim_Builds_On_ExternalId()
    {
        var command = new ExternalUserProfileBuild(Guid.NewGuid(), _claims, ExternalIdp.Google);
        var profile = await Mediator.Send(command);
        profile.Should().NotBeNull();
        profile.ExternalUserId.Should().Be(UsersData.EXTERNAL_ID);
    }

    [Fact]
    public async Task Facebook_Given_Email_Claim_Builds_On_ExternalId()
    {
        var command = new ExternalUserProfileBuild(Guid.NewGuid(), _claims, ExternalIdp.Facebook);
        var profile = await Mediator.Send(command);
        profile.Should().NotBeNull();
        profile.Email.Should().Be(UsersData.EMAIL);
    }

    [Fact]
    public async Task Google_Given_Email_Claim_Builds_On_ExternalId()
    {
        var command = new ExternalUserProfileBuild(Guid.NewGuid(), _claims, ExternalIdp.Google);
        var profile = await Mediator.Send(command);
        profile.Should().NotBeNull();
        profile.Email.Should().Be(UsersData.EMAIL);
    }

    [Fact]
    public async Task Facebook_Given_FullName_Claim_Builds_On_ExternalId()
    {
        var command = new ExternalUserProfileBuild(Guid.NewGuid(), _claims, ExternalIdp.Facebook);
        var profile = await Mediator.Send(command);
        profile.Should().NotBeNull();
        profile.FullName.Should().Be(UsersData.FULL_NAME);
    }

    [Fact]
    public async Task Google_Given_FullName_Claim_Builds_On_ExternalId()
    {
        var command = new ExternalUserProfileBuild(Guid.NewGuid(), _claims, ExternalIdp.Google);
        var profile = await Mediator.Send(command);
        profile.Should().NotBeNull();
        profile.FullName.Should().Be(UsersData.FULL_NAME);
    }

    [Fact]
    public async Task Facebook_Given_FirstName_Claim_Builds_On_ExternalId()
    {
        var command = new ExternalUserProfileBuild(Guid.NewGuid(), _claims, ExternalIdp.Facebook);
        var profile = await Mediator.Send(command);
        profile.Should().NotBeNull();
        profile.FirstName.Should().Be(UsersData.FIRST_NAME);
    }

    [Fact]
    public async Task Google_Given_FirstName_Claim_Builds_On_ExternalId()
    {
        var command = new ExternalUserProfileBuild(Guid.NewGuid(), _claims, ExternalIdp.Google);
        var profile = await Mediator.Send(command);
        profile.Should().NotBeNull();
        profile.FirstName.Should().Be(UsersData.FIRST_NAME);
    }

    [Fact]
    public async Task Facebook_Given_LastName_Claim_Builds_On_ExternalId()
    {
        var command = new ExternalUserProfileBuild(Guid.NewGuid(), _claims, ExternalIdp.Facebook);
        var profile = await Mediator.Send(command);
        profile.Should().NotBeNull();
        profile.LastName.Should().Be(UsersData.LAST_NAME);
    }

    [Fact]
    public async Task Google_Given_LastName_Claim_Builds_On_ExternalId()
    {
        var command = new ExternalUserProfileBuild(Guid.NewGuid(), _claims, ExternalIdp.Google);
        var profile = await Mediator.Send(command);
        profile.Should().NotBeNull();
        profile.LastName.Should().Be(UsersData.LAST_NAME);
    }
}