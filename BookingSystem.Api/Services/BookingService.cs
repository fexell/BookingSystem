using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;

namespace BookingSystem.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IResourceRepository _resourceRepository;

        public BookingService(IBookingRepository bookingRepository, IResourceRepository resourceRepository)
        {
            _bookingRepository = bookingRepository;
            _resourceRepository = resourceRepository;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            if(booking.StartTime.Minute != 0 || booking.EndTime.Minute != 0)
                throw new InvalidOperationException("Bookings must be whole hours.");

            var duration = booking.EndTime - booking.StartTime;
            if(duration.Minutes != 0 || duration.Seconds != 0 || duration.TotalHours < 1)
                throw new InvalidOperationException( "Bookings must be in whole hour increments (minimum 1 hour)." );
        
            var exisitingBookings = await _bookingRepository.GetByResourceIdAsync(booking.ResourceId);
            bool isOverlapping = exisitingBookings.Any( b =>
                b.Status == "Active" &&
                booking.StartTime < b.EndTime &&
                booking.EndTime > b.StartTime );

            if(isOverlapping)
                throw new InvalidOperationException( "Studio is already reserved at that time." );

            await _bookingRepository.AddAsync(booking);

            return booking;
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            await _bookingRepository.UpdateAsync(booking);
        }

        public async Task DeleteBookingAsync(int id)
        {
            await _bookingRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _bookingRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByResourceIdAsync(int resourceId)
        {
            return await _bookingRepository.GetByResourceIdAsync(resourceId);
        }
    }
}