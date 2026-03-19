using Microsoft.EntityFrameworkCore;
using BookingSystem.Api.Models;

namespace BookingSystem.Api.Repositories
{
    public interface IResourceRepository : IRepository<Resource>
    {
        Task<IEnumerable<Resource>> GetAvailableResourcesAsync();
    }
}
