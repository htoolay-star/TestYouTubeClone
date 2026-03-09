using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Domain.Services;
using YouTubeClone.Shared.DTOs;

namespace YouTubeClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await authService.LoginAsync(request);

            if (response == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(response);
        }

        [HttpGet("test-auth")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult TestAuth()
        {
            return Ok(new { message = "Authenticated successfully!" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await authService.RegisterAsync(request);
                if (!result) return BadRequest("Email is already taken.");

                return Ok(new { message = "Registration successful!" });
            }
            catch (Exception ex)
            {
                // Service ကနေ ပစ်လိုက်တဲ့ Exception message ကို ဒီမှာ ဖမ်းယူမယ်
                return StatusCode(500, ex.Message);
            }
        }
    }
}
