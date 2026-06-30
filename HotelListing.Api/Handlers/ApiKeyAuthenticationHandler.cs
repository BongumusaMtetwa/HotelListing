using System.Security.Claims;
using System.Text.Encodings.Web;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace HotelListing.Api.Handlers;

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> Options,
    ILoggerFactory _logger,
    UrlEncoder _encoder,
    IApiKeyValidatorService apiKeyValidatorService
) : AuthenticationHandler<AuthenticationSchemeOptions>(Options , _logger, _encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Try to retrieve the API key from the request headers using the specified header name (e.g., "X-API-KEY")
        var apiKey = string.Empty;

        if (!Request.Headers.TryGetValue(AuthenticationDefaults.ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            apiKey = apiKeyHeaderValues.ToString();
        }

        // If the API key is not present in the request headers, return an authentication failure result
        if (string.IsNullOrEmpty(apiKey))
        {
            return AuthenticateResult.NoResult();
        }

        // Validate the API key using the provided IApiKeyValidatorService. If the API key is invalid, return an authentication failure result
        var isValid = await apiKeyValidatorService.IsValidAsync(apiKey, Context.RequestAborted);
        if (!isValid)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }
       
       // If the API key is valid, create a ClaimsPrincipal with the appropriate claims and return a successful authentication result
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, apiKey),
            new Claim(ClaimTypes.Name, "ApiKeyClient")
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}