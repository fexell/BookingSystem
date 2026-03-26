using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BookingSystem.Client.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider {
    private readonly IJSRuntime _js;

    public CustomAuthStateProvider( IJSRuntime js ) {
        _js = js;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        // Read userId cookie
        var userId = await _js.InvokeAsync<string?>( "eval",
            "document.cookie.split(';').map(c=>c.trim()).find(c=>c.startsWith('userId='))?.split('=')[1] ?? null" );

        if ( string.IsNullOrEmpty( userId ) )
            return new AuthenticationState( new ClaimsPrincipal( new ClaimsIdentity() ) );

        // Read roles cookie (comma-separated)
        var rolesCookie = await _js.InvokeAsync<string?>( "eval",
            "document.cookie.split(';').map(c=>c.trim()).find(c=>c.startsWith('roles='))?.split('=')[1] ?? null" );

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        // Add roles if present
        if ( !string.IsNullOrEmpty( rolesCookie ) ) {
            var roles = rolesCookie.Split( ',', StringSplitOptions.RemoveEmptyEntries );
            foreach ( var role in roles )
                claims.Add( new Claim( ClaimTypes.Role, role ) );
        }

        var identity = new ClaimsIdentity( claims, "cookie" );
        return new AuthenticationState( new ClaimsPrincipal( identity ) );
    }

    public void NotifyAuthStateChanged() {
        NotifyAuthenticationStateChanged( GetAuthenticationStateAsync() );
    }
}
