using BookingSystem.Shared.DTOs;
using System.Net.Http.Json;

namespace BookingSystem.Client.Services;

public class ResourceService : IResourceService {
    private readonly IHttpClientFactory _httpClientFactory;

    public ResourceService( IHttpClientFactory httpClientFactory ) {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<ResourceDto>> GetAvailableResourcesAsync() {
        var client = _httpClientFactory.CreateClient( "API" );
        return await client.GetFromJsonAsync<List<ResourceDto>>( "api/resources/available" )
               ?? new List<ResourceDto>();
    }

    public async Task<ResourceDto?> GetResourceByIdAsync( int id ) {
        var client = _httpClientFactory.CreateClient( "API" );
        var response = await client.GetAsync( $"api/resources/{id}" );
        if ( !response.IsSuccessStatusCode ) return null;
        return await response.Content.ReadFromJsonAsync<ResourceDto>();
    }
}