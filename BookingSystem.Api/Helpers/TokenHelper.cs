

using System.IdentityModel.Tokens.Jwt;

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
}