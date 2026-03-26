using BookingSystem.Shared.DTOs;

namespace BookingSystem.Client.Services;

public interface IResourceService {
    Task<List<ResourceDto>> GetAvailableResourcesAsync();
    Task<ResourceDto?> GetResourceByIdAsync( int id );
}