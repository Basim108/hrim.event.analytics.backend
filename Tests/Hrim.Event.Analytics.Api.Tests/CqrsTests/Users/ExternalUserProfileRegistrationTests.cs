using System.Security.Claims;
using FluentAssertions;
using Hrim.Event.Analytics.Abstractions.Cqrs;
using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Users;
using Hrim.Event.Analytics.Api.Tests.Infrastructure;
using Hrim.Event.Analytics.Api.Tests.Infrastructure.AssertHelpers;
using Hrimsoft.Core.Extensions;
using NSubstitute;

namespace Hrim.Event.Analytics.Api.Tests.CqrsTests.Users;

public class ExternalUserProfileRegistrationTests: BaseCqrsTests
{
    private readonly UserProfileModel _profile;

    private DateTime _beforeSend = DateTime.UtcNow;

    public ExternalUserProfileRegistrationTests() {
        _profile = new UserProfileModel {
            FullName  = UsersData.FULL_NAME,
            FirstName = UsersData.FIRST_NAME,
            LastName  = UsersData.LAST_NAME
        };
    }

    [Fact]
    public async Task First_Login_Should_Register() {
        var claims = new List<Claim> {
            new("sub", $"facebook|{UsersData.EXTERNAL_ID}-new"),
            new("https://hrimsoft.us.auth0.com.example.com/email", UsersData.EMAIL + ".new")
        };
        OperatorContext = new OperationContext(claims, Guid.NewGuid());
        _apiRequestAccessor.GetUserClaims().Returns(claims);
        _apiRequestAccessor.GetOperationContext().Returns(OperatorContext);
        var command = new ExternalUserProfileRegistration(OperatorContext, _profile);

        var resultProfile = await Mediator.Send(command);

        resultProfile.CheckEntitySuccessfulCreation(_beforeSend);
        resultProfile.LastLogin.Should().BeAfter(_beforeSend);
        resultProfile.HrimUserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Second_Login_Should_Update_LastLogin() {
        var currentProfile = TestData.Users.CreateUniqueLogin();
        currentProfile.CheckEntitySuccessfulCreation();

        var command = new ExternalUserProfileRegistration(OperatorContext, _profile);
        _beforeSend = DateTime.UtcNow;

        var resultProfile = await Mediator.Send(command);

        resultProfile.UpdatedAt.Should().BeAfter(_beforeSend);
        resultProfile.IsDeleted.Should().BeNull();
        resultProfile.ConcurrentToken.Should().Be(2);
        resultProfile.LastLogin.Should().BeAfter(_beforeSend);
        resultProfile.ExternalUserId.Should().Be(UsersData.EXTERNAL_ID);
    }

    [Fact]
    public async Task Given_Different_Idp_But_Same_Email_Should_Link_Profile() {
        var userId = Guid.NewGuid();
        var email  = $"{userId}@mailinator.com";
        var user = TestData.Users.EnsureUserExistence(userId,
                                                      isDeleted: false,
                                                      externalId: Guid.NewGuid().ToString(),
                                                      idp: ExternalIdp.Facebook,
                                                      email);

        var googleUserId = Guid.NewGuid().ToString();
        var claims = new List<Claim> {
            new("sub", $"google|{googleUserId}"),
            new("https://hrimsoft.us.auth0.com.example.com/email", email)
        };
        OperatorContext = new OperationContext(claims, Guid.NewGuid());
        _apiRequestAccessor.GetUserClaims().Returns(claims);
        _apiRequestAccessor.GetOperationContext().Returns(OperatorContext);
        var command = new ExternalUserProfileRegistration(OperatorContext, _profile);
        _beforeSend = DateTime.UtcNow;

        var resultProfile = await Mediator.Send(command);

        // despite the fact that _profile is linked to a user, it's a new profile. 
        resultProfile.CheckEntitySuccessfulCreation(_beforeSend, userId);

        resultProfile.LastLogin.Should().BeAfter(_beforeSend);
        resultProfile.HrimUserId.Should().Be(userId);
        user.ExternalProfiles.Count.Should().Be(2);
    }

    [Fact]
    public async Task Given_Different_Email_For_Same_ExternalId_Should_Update_Email() {
        var userId     = Guid.NewGuid();
        var email      = $"{userId}@mailinator.com";
        var externalId = Guid.NewGuid().ToString();
        var user = TestData.Users.EnsureUserExistence(userId,
                                                      isDeleted: false,
                                                      externalId,
                                                      idp: ExternalIdp.Facebook,
                                                      $"{userId}@mailinator.com");
        var anotherProfile = user.ExternalProfiles.First();
        var claims = new List<Claim> {
            new("sub", $"facebook|{externalId}"),
            new("https://hrimsoft.us.auth0.com.example.com/email", email + ".new")
        };
        OperatorContext = new OperationContext(claims, Guid.NewGuid());
        _apiRequestAccessor.GetUserClaims().Returns(claims);
        _apiRequestAccessor.GetOperationContext().Returns(OperatorContext);
        var command       = new ExternalUserProfileRegistration(OperatorContext, _profile);
        var resultProfile = await Mediator.Send(command);

        resultProfile.CheckEntitySuccessfulUpdate(_beforeSend, userId, anotherProfile);

        resultProfile.LastLogin.Should().BeAfter(_beforeSend);
        resultProfile.Id.Should().Be(anotherProfile.Id);
        resultProfile.HrimUserId.Should().Be(user.Id);
        user.ExternalProfiles.Count.Should().Be(1);
        user.ExternalProfiles.Any(x => x.Id    == anotherProfile.Id).Should().BeTrue();
        user.ExternalProfiles.Any(x => x.Email == email + ".new").Should().BeTrue();
    }

    [Fact]
    public async Task Given_No_Email_Should_Find_User_By_ExternalId() {
        var userId     = Guid.NewGuid();
        var externalId = Guid.NewGuid().ToString();
        var user = TestData.Users.EnsureUserExistence(userId,
                                                      isDeleted: false,
                                                      externalId,
                                                      idp: ExternalIdp.Facebook,
                                                      email: null);
        var anotherProfile = user.ExternalProfiles.First();
        var claims = new List<Claim> {
            new("sub", $"facebook|{externalId}")
        };
        OperatorContext = new OperationContext(claims, Guid.NewGuid());
        _apiRequestAccessor.GetUserClaims().Returns(claims);
        _apiRequestAccessor.GetOperationContext().Returns(OperatorContext);
        var command       = new ExternalUserProfileRegistration(OperatorContext, _profile);
        var resultProfile = await Mediator.Send(command);

        resultProfile.CheckEntitySuccessfulUpdate(_beforeSend, userId, anotherProfile);

        resultProfile.LastLogin.Should().BeAfter(_beforeSend);
        resultProfile.Id.Should().Be(anotherProfile.Id);
        resultProfile.HrimUserId.Should().Be(user.Id);
        resultProfile.Email.Should().BeNull();
        user.ExternalProfiles.Count.Should().Be(1);
        user.ExternalProfiles.Any(x => x.Id             == anotherProfile.Id).Should().BeTrue();
        user.ExternalProfiles.Any(x => x.ExternalUserId == externalId).Should().BeTrue();
    }
    
    [Fact]
    public async Task Given_Google_Subject_Should_Register_Correct_Idp() {
        var externalId = Guid.NewGuid().ToString();
        var claims = new List<Claim> {
            new("sub", $"google-auth0|{externalId}")
        };
        OperatorContext = new OperationContext(claims, Guid.NewGuid());
        _apiRequestAccessor.GetUserClaims().Returns(claims);
        _apiRequestAccessor.GetOperationContext().Returns(OperatorContext);
        var command       = new ExternalUserProfileRegistration(OperatorContext, _profile);
        var resultProfile = await Mediator.Send(command);

        resultProfile.Idp.Should().Be(ExternalIdp.Google);
    }
}