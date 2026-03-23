using Microsoft.EntityFrameworkCore;
using BookingSystem.Api.Models;

namespace BookingSystem.Api.Repositories
{
    public interface IBookingRepository : IRepository<Booking>
    {
            Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
            Task<IEnumerable<Booking>> GetByResourceIdAsync(int resourceId);
    }
}
