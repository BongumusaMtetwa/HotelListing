using HotelListing.Api.Contracts;
using HotelListing.Api.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]

public class AuthController(IUsersService usersService) : BaseApiController
{
    
    [HttpPost("register")]
    public async Task<ActionResult<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
    {
        // var user = new ApplicationUser
        // {
        //     Email = registerUserDto.Email,
        //     UserName = registerUserDto.Email,
        //     FirstName = registerUserDto.FirstName,
        //     LastName = registerUserDto.LastName
        // };

        // var result = await userManager.CreateAsync(user, registerUserDto.Password);
        // if (!result.Succeeded)
        // {
        //     var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
        //     return MapErrorsToResponse(errors);
        // }
        // // Optinal: Send confirmation email or perform other post-registration actions here
        // return Ok(new { Message = "Registration successful." });
        var result = await usersService.RegisterAsync(registerUserDto);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>>Login(LoginUserDto loginUserDto)
    {
        // var user = await userManager.FindByEmailAsync(loginUserDto.Email);
        // if (user == null || !await userManager.CheckPasswordAsync(user, loginUserDto.Password))
        // {
        //     return Unauthorized(new { Message = "Invalid Credentials." });
        // }

        // var isPasswordValid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
        // if (!isPasswordValid)
        // {
        //     return Unauthorized(new { Message = "Invalid Credentials." });
        // }

        // // Optional: Generate JWT token or perform other login actions here
        // return Ok(new { Message = "Login successful." });
        var result = await usersService.LoginAsync(loginUserDto);
        return ToActionResult(result);
    }
}