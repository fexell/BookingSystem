using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

using BookingSystem.Shared.DTOs;

namespace BookingSystem.Client.Services;

public class AuthService : IAuthService {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly NavigationManager _navigation;

    public AuthService( IHttpClientFactory httpClientFactory, NavigationManager navigation ) {
        _httpClientFactory = httpClientFactory;
        _navigation = navigation;
    }

    // -------------------------
    // LOGIN
    // -------------------------
    public async Task<bool> LoginAsync( string email, string password ) {
        var client = _httpClientFactory.CreateClient( "API" );

        var response = await client.PostAsJsonAsync( "api/auth/login", new {
            email,
            password
        } );

        return response.IsSuccessStatusCode;
    }

    // -------------------------
    // REGISTER (returns backend error)
    // -------------------------
    public async Task<(bool Success, string? Error)> RegisterAsync( RegisterRequest request ) {
        var client = _httpClientFactory.CreateClient( "API" );
        var response = await client.PostAsJsonAsync( "api/auth/register", request );

        if ( response.IsSuccessStatusCode )
            return (true, null);

        // Try custom { message: "..."} format
        try {
            var customError = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if ( !string.IsNullOrWhiteSpace( customError?.Message ) )
                return (false, customError.Message);
        } catch { }

        // Try ModelState validation format
        try {
            var modelState = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            if ( modelState?.Errors != null ) {
                // Flatten first error message
                var firstError = modelState.Errors.Values.FirstOrDefault()?.FirstOrDefault();
                if ( !string.IsNullOrWhiteSpace( firstError ) )
                    return (false, firstError);
            }
        } catch { }

        return (false, "Ett okänt fel inträffade.");
    }

    private class ErrorResponse {
        public string Message { get; set; } = "";
    }

    // -------------------------
    // LOGOUT
    // -------------------------
    public async Task LogoutAsync() {
        var client = _httpClientFactory.CreateClient( "API" );
        await client.PostAsync( "api/auth/logout", null );
    }
}
