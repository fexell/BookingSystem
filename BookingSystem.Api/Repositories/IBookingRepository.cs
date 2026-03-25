using Microsoft.EntityFrameworkCore;
using BookingSystem.Api.Models;

namespace BookingSystem.Api.Repositories {
    public interface IBookingRepository : IRepository<Booking> {
        // Existing methods
        Task<IEnumerable<Booking>> GetByUserIdAsync( int userId );
        Task<IEnumerable<Booking>> GetByResourceIdAsync( int resourceId );

        // New methods that include User + Resource navigation properties
        Task<IEnumerable<Booking>> GetAllWithIncludesAsync();
        Task<Booking?> GetByIdWithIncludesAsync( int id );
        Task<IEnumerable<Booking>> GetByUserIdWithIncludesAsync( int userId );
        Task<IEnumerable<Booking>> GetByResourceIdWithIncludesAsync( int resourceId );
    }
}
