using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;

[ExcludeFromCodeCoverage]
public class TestAuthHandler: AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string NAME        = "Test user";
    public const string PICTURE_URI = "https://cdn.com/avatar.png";

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                           ILoggerFactory                               logger,
                           UrlEncoder                                   encoder,
                           ISystemClock                                 clock)
        : base(options: options, logger: logger, encoder: encoder, clock: clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        var claims = new[] {
            new Claim(type: "ClaimTypes.NameIdentifier",                       $"facebook|{UsersData.EXTERNAL_ID}"),
            new Claim(type: "https://hrimsoft.us.auth0.com.example.com/email", value: UsersData.EMAIL)
        };
        var identity  = new ClaimsIdentity(claims: claims, authenticationType: "IntegrationTest");
        var principal = new ClaimsPrincipal(identity: identity);
        var ticket    = new AuthenticationTicket(principal: principal, authenticationScheme: "IntegrationTest");

        var result = AuthenticateResult.Success(ticket: ticket);

        return Task.FromResult(result: result);
    }
}