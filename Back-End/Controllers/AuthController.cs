using CurrencyExchange.Models;
using CurrencyExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            if (string.IsNullOrEmpty(login.Username) || 
                string.IsNullOrEmpty(login.Password) ||
                login.Username.Length > 100 ||
                login.Password.Length > 100)
            {
                return BadRequest(new { error = "Username and password are required" });
            }

            if (_userService.ValidateUser(login.Username, login.Password))
            {
                var token = _userService.GenerateJwtToken(login.Username);
                
                Response.Cookies.Append("token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });
                
                return Ok(new { });
            }

            return Unauthorized(new { error = "Invalid username or password" });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            
            return Ok(new { });
        }

        [Authorize]
        [HttpGet("verify")]
        public IActionResult Verify()
        {
            return Ok(new { });
        }
    }
}