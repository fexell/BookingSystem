using BookingSystem.Shared.DTOs;
using System.Net.Http.Json;
using static BookingSystem.Client.Pages.Admin.Studios;

namespace BookingSystem.Client.Services;

public class ResourceService : IResourceService {
    private readonly IHttpClientFactory _httpClientFactory;

    public ResourceService( IHttpClientFactory httpClientFactory ) {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient( "API" );

    // ───────────────────────────────────────────────────────────────
    // PUBLIC ROUTES
    // ───────────────────────────────────────────────────────────────

    public async Task<List<ResourceDto>> GetAvailableResourcesAsync() {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<ResourceDto>>( "api/resources/available" )
               ?? new List<ResourceDto>();
    }

    public async Task<ResourceDto?> GetResourceByIdAsync( int id ) {
        var client = CreateClient();
        return await client.GetFromJsonAsync<ResourceDto>( $"api/resources/{id}" );
    }

    // ───────────────────────────────────────────────────────────────
    // ADMIN ROUTES
    // ───────────────────────────────────────────────────────────────

    public async Task<List<ResourceDto>> GetAllResourcesAsync() {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<ResourceDto>>( "api/resources" )
               ?? new List<ResourceDto>();
    }

    public async Task<bool> CreateResourceAsync( StudioEditModel model ) {
        var client = CreateClient();

        var payload = new {
            Name = model.Name,
            Type = model.Type,
            Description = model.Description,
            IsAvailable = true
        };

        var response = await client.PostAsJsonAsync( "api/resources", payload );
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateResourceAsync( int id, StudioEditModel model ) {
        var client = CreateClient();

        var payload = new {
            Id = id,
            Name = model.Name,
            Type = model.Type,
            Description = model.Description,
            IsAvailable = true // backend requires this field
        };

        var response = await client.PutAsJsonAsync( $"api/resources/{id}", payload );
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteResourceAsync( int id ) {
        var client = CreateClient();
        var response = await client.DeleteAsync( $"api/resources/{id}" );
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ToggleStatusAsync( int id ) {
        var client = CreateClient();

        // PATCH is not built-in, so we send an empty HttpRequestMessage
        var request = new HttpRequestMessage( HttpMethod.Patch, $"api/resources/{id}/toggle" );
        var response = await client.SendAsync( request );

        return response.IsSuccessStatusCode;
    }
}
