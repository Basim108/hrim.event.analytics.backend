using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
/// Provides control over authentication sessions for event analytics web applications 
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthController: ControllerBase {
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <inheritdoc />
    public AuthController(IHttpContextAccessor httpContextAccessor) {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary> Authenticate with the Facebook </summary>
    [HttpGet("facebook/login")]
    [Authorize(AuthenticationSchemes = FacebookDefaults.AuthenticationScheme)]
    public ActionResult FacebookLogin(string returnUri) => Redirect(returnUri);

    /// <summary> Authenticate with Google </summary>
    [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
    [HttpGet("google/login")]
    public ActionResult GoogleLogin(string returnUri) => Redirect(returnUri);

    /// <summary> Invoke user logout </summary>
    [HttpGet("logout")]
    [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = FacebookDefaults.AuthenticationScheme)]
    public async Task<ActionResult> FacebookLogout(string returnUri) {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
            await httpContext.SignOutAsync();
        return Redirect(returnUri);
    }
}