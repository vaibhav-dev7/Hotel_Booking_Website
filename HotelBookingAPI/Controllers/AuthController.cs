using System;
using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Auth;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterUser(dto);
            return Created("", new { message = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST: /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginDto dto)
    {
        try
        {
            var result = await _authService.LoginUser(dto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
