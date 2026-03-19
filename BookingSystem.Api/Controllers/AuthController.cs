
using BookingSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
                return Ok(new { message = "Registration failed!", userId = user.Id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var token = await _authService.LoginAsync(request.Email, request.Password);
            if (token == null)
                return Unauthorized("Wrong email or password.");
            return Ok(new { token });
        }
    }

    public record RegisterRequest(string Username, string Email, string Password);
    public record LoginRequest(string Email, string Password);
}