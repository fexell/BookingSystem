using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

using BookingSystem.Api.Services;
using BookingSystem.Api.Helpers;
using Microsoft.AspNetCore.Authentication;

namespace BookingSystem.Api.Middlewares;

public class RefreshTokenMiddleware {
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public RefreshTokenMiddleware(RequestDelegate next, IConfiguration configuration) {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync( HttpContext context, IAuthService authService ) {
        var jwt = context.Request.Cookies[ "jwt" ];
        var refreshToken = context.Request.Cookies[ "refreshToken" ];

        // Attempt refresh if jwt is missing OR expired
        if ( !string.IsNullOrEmpty( refreshToken ) && ( string.IsNullOrEmpty( jwt ) || IsTokenExpired( jwt ) ) ) {
            var user = await authService.RefreshAsync( refreshToken );
            if ( user != null ) {
                var newAccessToken = authService.GenerateToken( user );
                var newRefreshToken = await authService.GenerateRefreshTokenAsync( user );

                context.Response.Cookies.Append( "jwt", newAccessToken, CookieHelper.GetCookieOptions() );
                context.Response.Cookies.Append( "refreshToken", newRefreshToken, CookieHelper.GetCookieOptions() );
                
                // Inject the new access token into the request
                context.Request.Headers[ "Authorization" ] = $"Bearer {newAccessToken}";

                var result = await context.AuthenticateAsync( JwtBearerDefaults.AuthenticationScheme );
                if ( result.Succeeded )
                    context.User = result.Principal;
            }
        }

        await _next( context );
    }

    private bool IsTokenExpired( string jwt ) {
        try {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken( jwt );
            return token.ValidTo < DateTime.UtcNow;
        } catch {
            return true; // if we can't read it, treat it as expired
        }
    }
}