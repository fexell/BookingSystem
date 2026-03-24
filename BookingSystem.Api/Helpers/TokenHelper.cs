

using BookingSystem.Api.Models;
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
            return true; // if we can't read it, treat it as expired
        }
    }

    public static string GenerateToken ( User user, IConfiguration _configuration ) {
        var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes( _configuration[ "Jwt:Key" ]! ) );
        var credentials = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );

        var token = new JwtSecurityToken(
            issuer: _configuration[ "Jwt:Issuer" ],
            audience: _configuration[ "Jwt:Audience" ],
            claims: BuildClaims( user ),
            expires: DateTime.UtcNow.AddMinutes( int.Parse( _configuration[ "Jwt:AccessTokenLifetime" ]! ) ),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken( token );
    }

    private static IEnumerable<Claim> BuildClaims ( User user ) => [
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email!),
        new(ClaimTypes.Role, "User")
    ];
}