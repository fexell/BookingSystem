using BookingSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly AppDbContext _context;

        public ResourceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resource>> GetAllAsync()
        {
            return await _context.Resources.ToListAsync();
        }

        public async Task<Resource?> GetByIdAsync(int id)
        {
            return await _context.Resources
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Resource resource)
        {
            await _context.Resources.AddAsync(resource);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Resource resource)
        {
            _context.Resources.Update(resource);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var resource = await GetByIdAsync(id);
            if (resource != null)
            {
                _context.Resources.Remove(resource);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Resource>> GetAvailableResourcesAsync()
        {
            return await _context.Resources
                .Where(r => r.IsAvailable == true)
                .ToListAsync();
        }
    }
}
