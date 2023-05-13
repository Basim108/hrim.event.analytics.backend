using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Hrim user profile endpoints </summary>
[ApiController]
[Route(template: "v1/user-profile")]
public class UserProfileController: ControllerBase
{
    private readonly IApiRequestAccessor _accessor;
    private readonly IMediator           _mediator;

    /// <summary> </summary>
    public UserProfileController(IMediator mediator, IApiRequestAccessor accessor) {
        _mediator = mediator;
        _accessor = accessor;
    }

    /// <summary>
    ///     Access to user profile built for a user from authorization context
    /// </summary>
    [HttpPost(template: "me")]
    [Authorize]
    public async Task<IActionResult> RegisterMeAsync(UserProfileModel userProfile, CancellationToken cancellation) {
        var operationContext = _accessor.GetOperationContext();
        await _mediator.Send(new ExternalUserProfileRegistration(Context: operationContext, Profile: userProfile), cancellationToken: cancellation);
        return NoContent();
    }
}