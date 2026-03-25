using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace BookingSystem.Client.Services;

public class AuthService : IAuthService {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly NavigationManager _navigation;

    public AuthService( IHttpClientFactory httpClientFactory, NavigationManager navigation ) {
        _httpClientFactory = httpClientFactory;
        _navigation = navigation;
    }

    public async Task<bool> LoginAsync( string email, string password ) {
        var client = _httpClientFactory.CreateClient( "API" );
        var response = await client.PostAsJsonAsync( "api/auth/login", new {
            email,
            password
        } );
        return response.IsSuccessStatusCode;
    }

    public async Task LogoutAsync() {
        var client = _httpClientFactory.CreateClient( "API" );
        await client.PostAsync( "api/auth/logout", null );
    }
}