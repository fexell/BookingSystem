using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;

namespace BookingSystem.Api.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IResourceRepository _resourceRepository;

        public ResourceService(IResourceRepository resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        public async Task<IEnumerable<Resource>> GetAllResourcesAsync()
        {
            return await _resourceRepository.GetAllAsync();
        }

        public async Task<Resource?> GetResourceByIdAsync(int id)
        {
            return await _resourceRepository.GetByIdAsync(id);
        }

        public async Task<Resource> CreateResourceAsync(Resource resource)
        {
            await _resourceRepository.AddAsync(resource);
            return resource;
        }

        public async Task UpdateResourceAsync(Resource resource)
        {
            await _resourceRepository.UpdateAsync(resource);
        }

        public async Task DeleteResourceAsync(int id)
        {
            await _resourceRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Resource>> GetAvailableResourcesAsync()
        {
            return await _resourceRepository.GetAvailableResourcesAsync();
        }
    }
}