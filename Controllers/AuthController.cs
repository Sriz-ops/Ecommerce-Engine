using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // post/api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDto dto)
        {
            try
            {
                var customerId =
                    await _authService.RegisterAsync(dto);

                return Created(
                    $"api/auth/{customerId}",
                    new
                    {
                        CustomerId = customerId,
                        Message = "Registration successful."
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    Message = ex.Message
                });
            }
        }

        // post/api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto dto)
        {
            var result =
                await _authService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            return Ok(result);
        }

        // post /api/auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshTokenDto dto)
        {
            var result =
                await _authService.RefreshTokenAsync(
                    dto.RefreshToken);

            if (result == null)
            {
                return Unauthorized(new
                {
                    Message = "Invalid or expired refresh token."
                });
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
    [FromBody] RefreshTokenDto dto)
        {
            await _authService.LogoutAsync(
                dto.RefreshToken);

            return Ok(new
            {
                Message = "Logged out successfully."
            });
        }
    }
}