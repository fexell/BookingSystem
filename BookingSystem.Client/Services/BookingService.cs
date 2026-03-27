using BookingSystem.Shared.DTOs;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace BookingSystem.Client.Services;

public class BookingService : IBookingService {
    private readonly IHttpClientFactory _httpClientFactory;

    public BookingService( IHttpClientFactory httpClientFactory ) {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient( "API" );

    // ───────────────────────────────────────────────────────────────
    // CREATE BOOKING (already implemented)
    // ───────────────────────────────────────────────────────────────
    public async Task<BookingResult> CreateBookingAsync( CreateBookingRequest request ) {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync( "api/bookings", request );

        if ( response.IsSuccessStatusCode )
            return new BookingResult { Success = true };

        // Try to read structured JSON
        ErrorResponse? errorDto = null;

        try {
            errorDto = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        } catch {
            // fallback if backend didn't return JSON
        }

        return new BookingResult {
            Success = false,
            ErrorMessage = errorDto?.Message ?? "Unknown error occurred."
        };
    }

    // ───────────────────────────────────────────────────────────────
    // GET ALL BOOKINGS (Admin)
    // ───────────────────────────────────────────────────────────────
    public async Task<List<BookingDto>> GetAllBookingsAsync() {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<BookingDto>>( "api/bookings" )
               ?? new List<BookingDto>();
    }

    // ───────────────────────────────────────────────────────────────
    // DELETE BOOKING (Admin)
    // ───────────────────────────────────────────────────────────────
    public async Task<bool> DeleteBookingAsync( int id ) {
        var client = CreateClient();
        var response = await client.DeleteAsync( $"api/bookings/{id}" );
        return response.IsSuccessStatusCode;
    }
}
