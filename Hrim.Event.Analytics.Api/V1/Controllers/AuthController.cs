﻿using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

/// <summary>
///     Provides control over authentication sessions for event analytics web applications
/// </summary>
[ExcludeFromCodeCoverage]
[ApiController]
[Route(template: "[controller]")]
public class AuthController: ControllerBase
{
    private readonly IConfiguration          _appConfig;
    private readonly IHttpContextAccessor    _httpContextAccessor;
    private readonly ILogger<AuthController> _logger;

    /// <inheritdoc />
    public AuthController(IHttpContextAccessor    httpContextAccessor,
                          ILogger<AuthController> logger,
                          IConfiguration          appConfig) {
        _httpContextAccessor = httpContextAccessor;
        _logger              = logger;
        _appConfig           = appConfig;
    }

    /// <summary> Invoke user logout </summary>
    [HttpGet(template: "logout")]
    [Authorize]
    public async Task<ActionResult> LogoutAsync(string returnUri) {
        await _httpContextAccessor.HttpContext?.SignOutAsync()!;
        return ProcessRedirectToReturnUriAsync(userReturnUri: returnUri);
    }

    private ActionResult ProcessRedirectToReturnUriAsync(string userReturnUri) {
        if (string.IsNullOrWhiteSpace(value: userReturnUri))
            return Ok();
        try {
            var whiteList = _appConfig[key: "ALLOWED_ORIGINS"];
            if (!string.IsNullOrEmpty(value: whiteList)) {
                var returnUri = new Uri(uriString: userReturnUri);
                var isAllowed = whiteList.Split(separator: ";", options: StringSplitOptions.RemoveEmptyEntries)
                                         .Select(str => new Uri(uriString: str))
                                         .Any(allowedOrigin => allowedOrigin.Host == returnUri.Host && allowedOrigin.Port == returnUri.Port);
                if (isAllowed)
                    return Redirect(returnUri.ToString());
            }
        }
        catch (UriFormatException ex) {
            _logger.LogError(message: ApiLogs.RETURN_URI_IS_IN_WRONG_FORMAT, ex);
        }
        catch (Exception ex) {
            _logger.LogError(message: ApiLogs.RETURN_URI_PROCESSING_ERROR, ex);
        }
        return StatusCode((int)HttpStatusCode.Forbidden, value: ApiLogs.RETURN_URI_IS_NOT_ALLOWED);
    }
}