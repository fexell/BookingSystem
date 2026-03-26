using BookingSystem.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookingSystem.Api.Helpers;

public static class TokenHelper {
    public static bool IsTokenExpired( string jwt ) {
        try {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken( jwt );
            return token.ValidTo < DateTime.UtcNow;
        } catch {
            return true;
        }
    }

    public static async Task<string> GenerateToken(
        User user,
        UserManager<User> userManager,
        IConfiguration configuration ) {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes( configuration[ "Jwt:Key" ]! )
        );

        var credentials = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

        var claims = await BuildClaims( user, userManager );

        var token = new JwtSecurityToken(
            issuer: configuration[ "Jwt:Issuer" ],
            audience: configuration[ "Jwt:Audience" ],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse( configuration[ "Jwt:AccessTokenLifetime" ]! )
            ),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken( token );
    }

    private static async Task<IEnumerable<Claim>> BuildClaims(
        User user,
        UserManager<User> userManager ) {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!)
        };

        var roles = await userManager.GetRolesAsync( user );

        foreach ( var role in roles )
            claims.Add( new Claim( ClaimTypes.Role, role ) );

        return claims;
    }
}
