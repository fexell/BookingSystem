using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using BookingSystem.Api.Services;
using BookingSystem.Api.Filters;
using BookingSystem.Api.Helpers;
using BookingSystem.Api.Models;

// Hello
namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            IAuthService authService,
            UserManager<User> userManager,
            IConfiguration configuration)
        {
            _authService = authService;
            _userManager = userManager;
            _configuration = configuration;
        }

        // Kolla om användaren inte är inloggad, då kan vi låta dom registrera sig
        [ServiceFilter( typeof( NotLoggedInFilter ) )]
        [HttpPost( "register" )]
        public async Task<IActionResult> Register( RegisterRequest request ) {
            // If model validation failed, return ONE error
            if ( !ModelState.IsValid ) {
                var firstError = ModelState.Values
                    .SelectMany( v => v.Errors )
                    .Select( e => e.ErrorMessage )
                    .FirstOrDefault();

                return BadRequest( new { message = firstError } );
            }

            try {
                var user = await _authService.RegisterAsync(
                    request.Username,
                    request.FirstName,
                    request.Surname,
                    request.Email,
                    request.Password
                );

                return CreatedAtAction( nameof( Register ), new {
                    message = "Registration successful!",
                    userId = user.Id
                } );
            } catch ( InvalidOperationException ex ) {
                return BadRequest( new { message = ex.Message } );
            }
        }

        // Kolla om användaren inte är inloggad, då kan vi låta dom logga in
        [ServiceFilter( typeof( NotLoggedInFilter ) )]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _authService.LoginAsync( request.Email, request.Password );
            if ( user == null )
                return Unauthorized( new { message = "Wrong credentials!" } );

            var accessToken = await _authService.GenerateTokenAsync( user );
            var refreshToken = await _authService.GenerateRefreshTokenAsync( user );

            var roles = await _userManager.GetRolesAsync( user );

            Response.Cookies.Append( "jwt", accessToken, CookieHelper.GetCookieOptions() );
            Response.Cookies.Append( "refreshToken", refreshToken, CookieHelper.GetCookieOptions() );
            Response.Cookies.Append( "userId", user.Id.ToString(), CookieHelper.GetCookieOptions( httpOnly: false ) );
            Response.Cookies.Append( "roles", string.Join( ",", roles ), CookieHelper.GetCookieOptions( httpOnly: false ) );

            return Ok( new { userId = user.Id, message = "Login successful!" } );
        }

        // Om användaren är verkligen inloggad så kan vi låta dom logga ut
        [Authorize]
        [HttpPost( "logout" )]
        public async Task<IActionResult> Logout () {
            var refreshToken = Request.Cookies[ "refreshToken" ];
            if ( refreshToken != null )
                await _authService.RevokeRefreshTokenAsync( refreshToken );

            Response.Cookies.Delete( "jwt" );
            Response.Cookies.Delete( "refreshToken" );
            Response.Cookies.Delete( "userId" );
            return Ok( new { message = "Logged out successfully!" } );
        }
    }

    public record RegisterRequest(
        [Required] string Username,
        [Required, MinLength( 2 )] string FirstName,
        [Required, MinLength( 2 )] string Surname,
        [Required, EmailAddress] string Email,
        [Required] string Password);

    public record LoginRequest(
        [Required, EmailAddress] string Email,
        [Required] string Password);
}