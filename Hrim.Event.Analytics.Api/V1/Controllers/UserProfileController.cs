using System.Security.Claims;
using Hrim.Event.Analytics.Abstractions.Cqrs.Users;
using Hrim.Event.Analytics.Abstractions.ViewModels.Entities.Users;
using Hrim.Event.Analytics.Api.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers; 

/// <summary> Hrim user profile endpoints </summary>
[ApiController]
[Authorize]
[Route("v1/user-profile")]
public class UserProfileController: ControllerBase {
    
    /// <summary>
    /// Access to user profile built for a user from authorization context
    /// </summary>
    [HttpGet("me")]
    public ViewHrimUser GetMeAsync() {
        var fullName   = "";
        var pictureUri = "";
        foreach (var claim in User.Claims) {
            switch (claim.Type) {
                case ClaimTypes.Name:
                    fullName = claim.Value;
                    break;
                case HrimClaims.PICTURE:
                    pictureUri = claim.Value;
                    break;
            }
        }
        return new ViewHrimUser(fullName, pictureUri);
    }
}