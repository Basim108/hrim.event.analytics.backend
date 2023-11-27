using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.Controllers;

/// <summary>
/// Jobs Dashboard home controller
/// </summary>
[ExcludeFromCodeCoverage]
[Route("")]
[Authorize]
public class HomeController: ControllerBase
{
    /// <summary> </summary>
    [HttpGet("")]
    public IActionResult Index() { return Redirect("/jobs"); }

    /// <summary> </summary>
    [HttpGet("logout")]
    public async Task Logout()
    {
        await Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await Request.HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    }
}