using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Users;

public class ExternalUserProfileRegistrationTests : BaseCqrsTests
{
    private readonly ExternalUserProfile _profile;

    public ExternalUserProfileRegistrationTests()
    {
        _profile = new ExternalUserProfile
        {
            ExternalUserId = UsersData.EXTERNAL_ID,
            Email = UsersData.EMAIL,
            FullName = UsersData.FULL_NAME,
            FirstName = UsersData.FIRST_NAME,
            LastName = UsersData.LAST_NAME
        };
    }

    [Fact]
    public async Task First_Login_Should_Register()
    {
        var command = new ExternalUserProfileRegistration(Guid.NewGuid(), _profile);
        var beforeSend = DateTime.UtcNow;

        var resultProfile = await Mediator.Send(command);

        resultProfile.CheckEntitySuccessfulCreation(beforeSend);
        resultProfile.LastLogin.Should().BeAfter(beforeSend);
        resultProfile.HrimUserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Second_Login_Should_Update_LastLogin()
    {
        var currentProfile = TestData.Users.CreateUniqueLogin();
        currentProfile.CheckEntitySuccessfulCreation();

        var command = new ExternalUserProfileRegistration(Guid.NewGuid(), currentProfile);
        var beforeSend = DateTime.UtcNow;

        var resultProfile = await Mediator.Send(command);

        resultProfile.CheckEntitySuccessfulUpdate(beforeSend, null, currentProfile);
        resultProfile.LastLogin.Should().BeAfter(beforeSend);
        resultProfile.HrimUserId.Should().Be(currentProfile.HrimUserId);
    }

    [Fact]
    public async Task Given_Different_Idp_But_Same_Email_Should_Link_Profile()
    {
        var user = TestData.Users.EnsureUserExistence(Guid.NewGuid());
        var email = $"{user.Id}@mailinator.com";
        var anotherProfile = TestData.Users.CreateUniqueLogin(user.Id, email, idp: ExternalIdp.Facebook);
        anotherProfile.CheckEntitySuccessfulCreation();

        _profile.Email = email;
        _profile.ExternalUserId = Guid.NewGuid().ToString();
        _profile.Idp = ExternalIdp.Google;

        var command = new ExternalUserProfileRegistration(Guid.NewGuid(), _profile);
        var beforeSend = DateTime.UtcNow;

        var resultProfile = await Mediator.Send(command);

        // despite the fact that _profile is linked to a user, it's a new profile. 
        resultProfile.CheckEntitySuccessfulCreation(beforeSend, null);

        resultProfile.LastLogin.Should().BeAfter(beforeSend);
        resultProfile.HrimUserId.Should().Be(user.Id);
        user.ExternalProfiles.Count.Should().Be(2);
        user.ExternalProfiles.Any(x => x.Id == anotherProfile.Id).Should().BeTrue();
        user.ExternalProfiles.Any(x => x.Id == _profile.Id).Should().BeTrue();
    }

    [Fact]
    public async Task Given_Different_Email_For_Same_ExternalId_Should_Update_Email()
    {
        var user = TestData.Users.EnsureUserExistence(Guid.NewGuid());
        var externalId = Guid.NewGuid().ToString();
        var anotherProfile = TestData.Users.CreateUniqueLogin(user.Id, externalId: externalId, idp: ExternalIdp.Google);
        anotherProfile.CheckEntitySuccessfulCreation();

        _profile.Email = $"new-{externalId}@mailinator.com";
        _profile.ExternalUserId = externalId;
        _profile.Idp = ExternalIdp.Google;

        var command = new ExternalUserProfileRegistration(Guid.NewGuid(), _profile);
        var beforeSend = DateTime.UtcNow;

        var resultProfile = await Mediator.Send(command);

        resultProfile.CheckEntitySuccessfulUpdate(beforeSend, null, anotherProfile);

        resultProfile.LastLogin.Should().BeAfter(beforeSend);
        resultProfile.Id.Should().Be(anotherProfile.Id);
        resultProfile.HrimUserId.Should().Be(user.Id);
        user.ExternalProfiles.Count.Should().Be(1);
        user.ExternalProfiles.Any(x => x.Id == anotherProfile.Id).Should().BeTrue();
    }
}