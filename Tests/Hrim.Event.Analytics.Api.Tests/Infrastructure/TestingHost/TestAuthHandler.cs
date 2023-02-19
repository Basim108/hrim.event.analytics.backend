using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Hrim.Event.Analytics.Api.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;

[ExcludeFromCodeCoverage]
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string NAME = "Test user";
    public const string PICTURE_URI = "https://cdn.com/avatar.png";

    public static readonly Guid UserId = Guid.Parse("d46d580f-7b45-4b2b-95b0-1c523f68d3eb");
    public static readonly string Email = $"test-{UserId}@mailinator.com";

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, NAME),
            new Claim(ClaimTypes.Email, Email),
            new Claim(HrimClaims.PICTURE, PICTURE_URI),
            new Claim(HrimClaims.HRIM_USER_ID, UserId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "IntegrationTest");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "IntegrationTest");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}