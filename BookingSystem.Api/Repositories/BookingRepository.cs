using BookingSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Repositories {
    public class BookingRepository : IBookingRepository {
        private readonly AppDbContext _context;

        public BookingRepository( AppDbContext context ) {
            _context = context;
        }

        // -----------------------------
        // Base CRUD (from IRepository)
        // -----------------------------

        public async Task<IEnumerable<Booking>> GetAllAsync() {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync( int id ) {
            return await _context.Bookings.FirstOrDefaultAsync( b => b.Id == id );
        }

        public async Task AddAsync( Booking booking ) {
            await _context.Bookings.AddAsync( booking );
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync( Booking booking ) {
            _context.Bookings.Update( booking );
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync( int id ) {
            var booking = await GetByIdAsync( id );
            if ( booking != null ) {
                _context.Bookings.Remove( booking );
                await _context.SaveChangesAsync();
            }
        }

        // -----------------------------
        // Queries WITHOUT includes
        // (used internally for validation)
        // -----------------------------

        public async Task<IEnumerable<Booking>> GetByUserIdAsync( int userId ) {
            return await _context.Bookings
                .Where( b => b.UserId == userId )
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByResourceIdAsync( int resourceId ) {
            return await _context.Bookings
                .Where( b => b.ResourceId == resourceId )
                .ToListAsync();
        }

        // -----------------------------
        // Queries WITH includes
        // (used by service → mapper → DTO)
        // -----------------------------

        public async Task<IEnumerable<Booking>> GetAllWithIncludesAsync() {
            return await _context.Bookings
                .Include( b => b.User )
                .Include( b => b.Resource )
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdWithIncludesAsync( int id ) {
            return await _context.Bookings
                .Include( b => b.User )
                .Include( b => b.Resource )
                .FirstOrDefaultAsync( b => b.Id == id );
        }

        public async Task<IEnumerable<Booking>> GetByUserIdWithIncludesAsync( int userId ) {
            return await _context.Bookings
                .Where( b => b.UserId == userId )
                .Include( b => b.User )
                .Include( b => b.Resource )
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByResourceIdWithIncludesAsync( int resourceId ) {
            return await _context.Bookings
                .Where( b => b.ResourceId == resourceId )
                .Include( b => b.User )
                .Include( b => b.Resource )
                .ToListAsync();
        }
    }
}
