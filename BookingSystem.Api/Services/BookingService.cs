using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;

namespace BookingSystem.Api.Services {
    public class BookingService : IBookingService {
        private readonly IBookingRepository _bookingRepository;
        private readonly IResourceRepository _resourceRepository;

        public BookingService( IBookingRepository bookingRepository, IResourceRepository resourceRepository ) {
            _bookingRepository = bookingRepository;
            _resourceRepository = resourceRepository;
        }

        // Return fully-loaded bookings (User + Resource)
        public async Task<IEnumerable<Booking>> GetAllBookingsAsync() {
            return await _bookingRepository.GetAllWithIncludesAsync();
        }

        // Return fully-loaded booking (User + Resource)
        public async Task<Booking?> GetBookingByIdAsync( int id ) {
            return await _bookingRepository.GetByIdWithIncludesAsync( id );
        }

        public async Task<Booking> CreateBookingAsync( Booking booking ) {
            // --- Validation rules ---
            if ( booking.StartTime.Minute != 0 || booking.EndTime.Minute != 0 )
                throw new InvalidOperationException( "Bookings must be whole hours." );

            var duration = booking.EndTime - booking.StartTime;
            if ( duration.Minutes != 0 || duration.Seconds != 0 || duration.TotalHours < 1 )
                throw new InvalidOperationException( "Bookings must be in whole hour increments (minimum 1 hour)." );

            // Use lightweight query (no includes) for overlap validation
            var existingBookings = await _bookingRepository.GetByResourceIdAsync( booking.ResourceId );

            bool isOverlapping = existingBookings.Any( b =>
                b.Status == "Active" &&
                booking.StartTime < b.EndTime &&
                booking.EndTime > b.StartTime );

            if ( isOverlapping )
                throw new InvalidOperationException( "Studio is already reserved at that time." );

            // Save booking
            await _bookingRepository.AddAsync( booking );

            // Return fully-loaded version for API response
            return await _bookingRepository.GetByIdWithIncludesAsync( booking.Id )
                   ?? booking;
        }

        public async Task UpdateBookingAsync( Booking booking ) {
            await _bookingRepository.UpdateAsync( booking );
        }

        public async Task DeleteBookingAsync( int id ) {
            await _bookingRepository.DeleteAsync( id );
        }

        // Fully-loaded bookings for user
        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync( int userId ) {
            return await _bookingRepository.GetByUserIdWithIncludesAsync( userId );
        }

        // Fully-loaded bookings for resource
        public async Task<IEnumerable<Booking>> GetBookingsByResourceIdAsync( int resourceId ) {
            return await _bookingRepository.GetByResourceIdWithIncludesAsync( resourceId );
        }
    }
}
