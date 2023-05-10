using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.Services;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Users;
using Hrim.Event.Analytics.Api.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary> Hrim user profile endpoints </summary>
[ApiController]
[Route("v1/user-profile")]
public class UserProfileController: ControllerBase
{
    private readonly IMediator           _mediator;
    private readonly IApiRequestAccessor _accessor;

    /// <summary> </summary>
    public UserProfileController(IMediator mediator, IApiRequestAccessor accessor) {
        _mediator = mediator;
        _accessor = accessor;
    }

    /// <summary>
    ///     Access to user profile built for a user from authorization context
    /// </summary>
    [HttpPost("me")]
    public async Task<IActionResult> RegisterMeAsync(UserProfileModel userProfile, CancellationToken cancellation) {
        var operationContext = _accessor.GetOperationContext();
        await _mediator.Send(new ExternalUserProfileRegistration(operationContext, userProfile), cancellation);
        return NoContent();
    }
}