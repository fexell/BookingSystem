using BookingSystem.Shared.DTOs;
using System.Net.Http.Json;

namespace BookingSystem.Client.Services;

public class BookingService : IBookingService {
    private readonly IHttpClientFactory _httpClientFactory;

    public BookingService( IHttpClientFactory httpClientFactory ) {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> CreateBookingAsync( CreateBookingRequest request ) {
        var client = _httpClientFactory.CreateClient( "API" );
        var response = await client.PostAsJsonAsync( "api/bookings", request );
        return response.IsSuccessStatusCode;
    }
}