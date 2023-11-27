using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure.TestingHost;

public class DisableAuthenticationPolicyEvaluator: IPolicyEvaluator
{
    public async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context) {
        // Always pass authentication.
        var claims = new[] {
            new Claim(type: "ClaimTypes.NameIdentifier",                       $"facebook|{UsersData.EXTERNAL_ID}"),
            new Claim(type: "https://hrimsoft.us.auth0.com.example.com/email", value: UsersData.EMAIL)
        };
        var identity  = new ClaimsIdentity(claims: claims, authenticationType: "IntegrationTest");
        var principal = new ClaimsPrincipal(identity: identity);
        var ticket    = new AuthenticationTicket(principal: principal, authenticationScheme: "IntegrationTest");

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }

    public async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
                                                                AuthenticateResult  authenticationResult,
                                                                HttpContext         context,
                                                                object?             resource) {
        // Always pass authorization
        return await Task.FromResult(PolicyAuthorizationResult.Success());
    }
}