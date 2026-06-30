using System.Security.Claims;
using System.Text.Encodings.Web;
using HotelListing.Api.Contracts;
using HotelListing.Api.DTOs.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace HotelListing.Api.Handlers;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> Options,
    ILoggerFactory _logger,
    UrlEncoder _encoder,
    IUsersService userService
) : AuthenticationHandler<AuthenticationSchemeOptions>(Options , _logger, _encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if(!Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }
    
        //Basic Base64(username:password)
        var authHeader = authHeaderValues.ToString();
        if(string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }
        
        // Extract the token from the Authorization header by removing the "Basic " prefix and trimming any whitespace
        var token = authHeader["Basic ".Length..].Trim();
        string decoded;

        try
        {
            // Decode the Base64-encoded token to retrieve the original credentials (username and password)
            var credentialBytes = Convert.FromBase64String(token);
            //{username:password}
            decoded = System.Text.Encoding.UTF8.GetString(credentialBytes);
        }
        catch
        {
            return AuthenticateResult.Fail("invalid basic authentication token");
        }

        var separatorIndex = decoded.IndexOf(':');
        if(separatorIndex <= 0)
        {
            return AuthenticateResult.Fail("invalid basic authentication credentials format.");
        }

        var userNameOrEmail = decoded[..separatorIndex];
        var password = decoded[(separatorIndex + 1)..];

        var loginDto = new LoginUserDto
        {
            Email = userNameOrEmail,
            Password = password
        };

        var result = await userService.LoginAsync(loginDto);
        if(!result.IsSuccess)
        {
            return AuthenticateResult.Fail("invalid username or password");
        }

        // Create a list of claims for the authenticated user, including the user's name (which can be either the username or email)
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userNameOrEmail)
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}