using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using BookingSystem.Api.Services;
using BookingSystem.Api.Helpers;

namespace BookingSystem.Api.Middlewares;

public class RefreshTokenMiddleware {
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public RefreshTokenMiddleware(RequestDelegate next, IConfiguration configuration) {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, IAuthService authService) {
        var jwt = context.Request.Cookies[ "jwt" ];
        var refreshToken = context.Request.Cookies[ "refreshToken" ];

        if(string.IsNullOrEmpty(jwt) && !string.IsNullOrEmpty(refreshToken)) {
            var user = await authService.RefreshAsync( refreshToken );

            if(user != null) {
                var newAccessToken = authService.GenerateToken( user );
                var newRefreshToken = await authService.GenerateRefreshTokenAsync( user );

                context.Response.Cookies.Append( "jwt", newAccessToken, CookieHelper.GetCookieOptions() );
                context.Response.Cookies.Append( "refreshToken", newRefreshToken, CookieHelper.GetCookieOptions() );

                context.Request.Headers[ "Authorization" ] = $"Bearer {newAccessToken}";
            }
        }

        await _next( context );
    }
}