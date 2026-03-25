using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;
using BookingSystem.Shared.DTOs;

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

        private static BookingDto Map(Booking b) => new BookingDto
        {
            Id = b.Id,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            Status = b.Status,
            UserId = b.UserId,
            ResourceId = b.ResourceId
        };

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return bookings.Select(Map);
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            return booking != null ? Map(booking) : null;
        }

        public async Task<BookingDto> CreateBookingAsync(Booking booking)
        {
            // Kollar om den nya bokningen överlappar med en befintlig bokning för samma resurs
            var existingBookings = await _bookingRepository.GetByResourceIdAsync(booking.ResourceId);

            bool isOverlapping = existingBookings.Any(b =>
                b.Status == "Active" &&
                booking.StartTime < b.EndTime &&
                booking.EndTime > b.StartTime);

            if (isOverlapping)
                throw new InvalidOperationException("Resource is already booked during this time.");

            await _bookingRepository.AddAsync(booking);
            return Map(booking);
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            await _bookingRepository.UpdateAsync(booking);
        }

        public async Task DeleteBookingAsync(int id)
        {
            await _bookingRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(int userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return bookings.Select(Map);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByResourceIdAsync(int resourceId)
        {
            var bookings = await _bookingRepository.GetByResourceIdAsync(resourceId);
            return bookings.Select(Map);
        }
    }
}