using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Hrim.Event.Analytics.Api.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost
{
    [ExcludeFromCodeCoverage]
    public class TestAuthHandler: AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                               ILoggerFactory                               logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var userId = Guid.NewGuid().ToString();
            var claims    = new[] {
                new Claim(ClaimTypes.Name, "Test user"),
                new Claim(ClaimTypes.Email, $"test-{userId}@mailinator.com"),
                new Claim(HrimClaims.HRIM_USER_ID, userId)
            };
            var identity  = new ClaimsIdentity(claims, "IntegrationTest");
            var principal = new ClaimsPrincipal(identity);
            var ticket    = new AuthenticationTicket(principal, "IntegrationTest");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
