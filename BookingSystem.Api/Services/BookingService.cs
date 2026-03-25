using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;
using BookingSystem.Shared.DTOs;

namespace BookingSystem.Api.Services {
    public class BookingService : IBookingService {
        private readonly IBookingRepository _bookingRepository;
        private readonly IResourceRepository _resourceRepository;

        public BookingService( IBookingRepository bookingRepository, IResourceRepository resourceRepository ) {
            _bookingRepository = bookingRepository;
            _resourceRepository = resourceRepository;
        }

        // Helper mapper
        private static BookingDto ToDto( Booking b ) => new BookingDto {
            Id = b.Id,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            Status = b.Status,
            UserId = b.UserId,
            ResourceId = b.ResourceId
        };

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync() {
            var bookings = await _bookingRepository.GetAllWithIncludesAsync();
            return bookings.Select( ToDto );
        }

        public async Task<BookingDto?> GetBookingByIdAsync( int id ) {
            var booking = await _bookingRepository.GetByIdWithIncludesAsync( id );
            return booking is null ? null : ToDto( booking );
        }

        public async Task<BookingDto> CreateBookingAsync( Booking booking ) {
            // --- Validation rules ---
            if ( booking.StartTime.Minute != 0 || booking.EndTime.Minute != 0 )
                throw new InvalidOperationException( "Bookings must be whole hours." );

            var duration = booking.EndTime - booking.StartTime;
            if ( duration.Minutes != 0 || duration.Seconds != 0 || duration.TotalHours < 1 )
                throw new InvalidOperationException( "Bookings must be in whole hour increments (minimum 1 hour)." );

            // Overlap validation
            var existingBookings = await _bookingRepository.GetByResourceIdAsync( booking.ResourceId );

            bool isOverlapping = existingBookings.Any( b =>
                b.Status == "Active" &&
                booking.StartTime < b.EndTime &&
                booking.EndTime > b.StartTime );

            if ( isOverlapping )
                throw new InvalidOperationException( "Studio is already reserved at that time." );

            // Save booking
            await _bookingRepository.AddAsync( booking );

            // Return fully-loaded version
            var saved = await _bookingRepository.GetByIdWithIncludesAsync( booking.Id ) ?? booking;
            return ToDto( saved );
        }

        public async Task UpdateBookingAsync( Booking booking ) {
            await _bookingRepository.UpdateAsync( booking );
        }

        public async Task DeleteBookingAsync( int id ) {
            await _bookingRepository.DeleteAsync( id );
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync( int userId ) {
            var bookings = await _bookingRepository.GetByUserIdWithIncludesAsync( userId );
            return bookings.Select( ToDto );
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByResourceIdAsync( int resourceId ) {
            var bookings = await _bookingRepository.GetByResourceIdWithIncludesAsync( resourceId );
            return bookings.Select( ToDto );
        }
    }
}
