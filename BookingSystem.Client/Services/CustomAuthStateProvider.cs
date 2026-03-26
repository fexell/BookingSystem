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
        // Read the userId cookie (non-httpOnly, so JS can access it)
        var userId = await _js.InvokeAsync<string?>( "eval",
            "document.cookie.split(';').map(c=>c.trim()).find(c=>c.startsWith('userId='))?.split('=')[1] ?? null" );

        if ( string.IsNullOrEmpty( userId ) )
            return new AuthenticationState( new ClaimsPrincipal( new ClaimsIdentity() ) );

        var claims = new[] { new Claim( ClaimTypes.NameIdentifier, userId ) };
        var identity = new ClaimsIdentity( claims, "cookie" );
        return new AuthenticationState( new ClaimsPrincipal( identity ) );
        }

    public void NotifyAuthStateChanged() {
        NotifyAuthenticationStateChanged( GetAuthenticationStateAsync() );
    }
}