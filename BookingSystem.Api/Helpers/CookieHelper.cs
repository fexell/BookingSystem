

namespace BookingSystem.Api.Helpers;

public class CookieHelper {
    public static CookieOptions GetCookieOptions( bool httpOnly = true ) => new CookieOptions {
        HttpOnly = httpOnly,
        Secure = true,
        SameSite = SameSiteMode.None,
        Expires = DateTimeOffset.UtcNow.AddHours( 8 )
    };
}