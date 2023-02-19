using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions;
using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Enums;
using Hrim.Event.Analytics.Abstractions.Exceptions;
using Hrim.Event.Analytics.Api.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Hrim.Event.Analytics.Api.Authentication;

/// <inheritdoc />
public class EventAnalyticsOAuthEvents : OAuthEvents
{
    private readonly IApiRequestAccessor _apiRequestAccessor;
    private readonly ILogger<EventAnalyticsOAuthEvents> _logger;
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public EventAnalyticsOAuthEvents(ILogger<EventAnalyticsOAuthEvents> logger,
        IApiRequestAccessor apiRequestAccessor,
        IMediator mediator)
    {
        _logger = logger;
        _apiRequestAccessor = apiRequestAccessor;
        _mediator = mediator;
    }

    /// <inheritdoc />
    public override async Task CreatingTicket(OAuthCreatingTicketContext context)
    {
        if (context.Principal == null ||
            context.Principal.Identity is not ClaimsIdentity identity)
        {
            await base.CreatingTicket(context);
            return;
        }

        if (!identity.IsAuthenticated)
        {
            _logger.LogDebug(ApiLogs.IDENTITY_IS_NOT_AUTHENTICATED);
            return;
        }

        using var externalIdScope = _logger.BeginScope(ApiLogs.EXTERNAL_USER_ID,
            identity.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
        using var authTypeScope = _logger.BeginScope(ApiLogs.AUTH_TYPE, identity.AuthenticationType);
        switch (identity.AuthenticationType)
        {
            case GoogleDefaults.AuthenticationScheme:
                await RegisterUserLoginAsync(identity, ExternalIdp.Google);
                break;
            case FacebookDefaults.AuthenticationScheme:
                await RegisterUserLoginAsync(identity, ExternalIdp.Facebook);
                FacebookAddPictureClaim(context, identity);
                _logger.LogDebug(ApiLogs.FB_PICTURE_WAS_ADDED);
                break;
            default:
                throw new UnsupportedAuthTypeException(identity.AuthenticationType);
        }

        await base.CreatingTicket(context);
    }

    private async Task RegisterUserLoginAsync(ClaimsIdentity identity, ExternalIdp idp)
    {
        _logger.LogDebug(CoreLogs.SERVICE_START_HANDLING, nameof(RegisterUserLoginAsync));
        var correlationId = _apiRequestAccessor.GetCorrelationId();
        var claimsDict = identity.Claims.ToDictionary(x => x.Type, x => x.Value);
        var userProfile = await _mediator.Send(new ExternalUserProfileBuild(correlationId, claimsDict, idp));
        var createdProfile = await _mediator.Send(new ExternalUserProfileRegistration(correlationId, userProfile));
        identity.AddClaim(new Claim(HrimClaims.HRIM_USER_ID, createdProfile.HrimUserId.ToString()));
        _logger.LogDebug(CoreLogs.SERVICE_FINISH_HANDLING, nameof(RegisterUserLoginAsync));
    }

    private static void FacebookAddPictureClaim(OAuthCreatingTicketContext context, ClaimsIdentity identity)
    {
        var pictureProp = context.User.GetProperty("picture");
        var dataProp = pictureProp.GetProperty("data");
        var profileImg = dataProp.GetString("url");
        if (!string.IsNullOrWhiteSpace(profileImg)) identity.AddClaim(new Claim(HrimClaims.PICTURE, profileImg));
    }
}