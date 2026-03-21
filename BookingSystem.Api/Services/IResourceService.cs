using BookingSystem.Api.Models;

namespace BookingSystem.Api.Services
{
    public interface IResourceService
    {
        Task<IEnumerable<Resource>> GetAllResourcesAsync();
        Task<Resource?> GetResourceByIdAsync(int id);
        Task<Resource> CreateResourceAsync(Resource resource);
        Task UpdateResourceAsync(Resource resource);
        Task DeleteResourceAsync(int id);
        Task<IEnumerable<Resource>> GetAvailableResourcesAsync();
    }
}