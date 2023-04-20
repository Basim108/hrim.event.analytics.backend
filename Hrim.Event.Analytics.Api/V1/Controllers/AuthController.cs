using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
///     Provides control over authentication sessions for event analytics web applications
/// </summary>
[ExcludeFromCodeCoverage]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _appConfig;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthController> _logger;

    /// <inheritdoc />
    public AuthController(IHttpContextAccessor httpContextAccessor,
        ILogger<AuthController> logger,
        IConfiguration appConfig)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _appConfig = appConfig;
    }

    /// <summary> Authenticate with the Facebook </summary>
    [HttpGet("facebook/login")]
    [Authorize(AuthenticationSchemes = FacebookDefaults.AuthenticationScheme)]
    public Task<ActionResult> FacebookLoginAsync(string returnUri)
    {
        return ProcessRedirectToReturnUriAsync(returnUri);
    }

    /// <summary> Authenticate with Google </summary>
    [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
    [HttpGet("google/login")]
    public Task<ActionResult> GoogleLoginAsync(string returnUri)
    {
        return ProcessRedirectToReturnUriAsync(returnUri);
    }

    /// <summary> Invoke user logout </summary>
    [HttpGet("logout")]
    [Authorize]
    public async Task<ActionResult> LogoutAsync(string returnUri)
    {
        await LogoutAsync();
        return await ProcessRedirectToReturnUriAsync(returnUri);
    }

    private async Task LogoutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
            await httpContext.SignOutAsync();
    }

    private async Task<ActionResult> ProcessRedirectToReturnUriAsync(string userReturnUri)
    {
        if (string.IsNullOrWhiteSpace(userReturnUri))
            return Ok();
        try
        {
            var whiteList = _appConfig["ALLOWED_ORIGINS"];
            if (!string.IsNullOrEmpty(whiteList))
            {
                var returnUri = new Uri(userReturnUri);
                var isAllowed = whiteList.Split(";", StringSplitOptions.RemoveEmptyEntries)
                    .Select(str => new Uri(str))
                    .Any(allowedOrigin => allowedOrigin.Host == returnUri.Host &&
                                          allowedOrigin.Port == returnUri.Port);
                if (isAllowed)
                    return Redirect(returnUri.ToString());
            }
        }
        catch (UriFormatException ex)
        {
            _logger.LogError(ApiLogs.RETURN_URI_IS_IN_WRONG_FORMAT, ex.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ApiLogs.RETURN_URI_PROCESSING_ERROR, ex.ToString());
        }

        if (User.Identity?.IsAuthenticated ?? false)
            await LogoutAsync();
        return StatusCode((int)HttpStatusCode.Forbidden, ApiLogs.RETURN_URI_IS_NOT_ALLOWED);
    }
}