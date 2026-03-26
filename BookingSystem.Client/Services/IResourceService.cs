using BookingSystem.Shared.DTOs;
using static BookingSystem.Client.Pages.Admin.Studios;

namespace BookingSystem.Client.Services;

public interface IResourceService {
    Task<List<ResourceDto>> GetAvailableResourcesAsync();
    Task<ResourceDto?> GetResourceByIdAsync( int id );
    Task<bool> CreateResourceAsync( StudioEditModel model );
    Task<bool> UpdateResourceAsync( int id, StudioEditModel model );
    Task<bool> DeleteResourceAsync( int id );
    Task<bool> ToggleStatusAsync( int id );
    Task<List<ResourceDto>> GetAllResourcesAsync();
}