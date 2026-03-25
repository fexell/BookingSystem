using BookingSystem.Api.Models;
using BookingSystem.Shared.DTOs;

namespace BookingSystem.Api.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto?> GetBookingByIdAsync(int id);
        Task<BookingDto> CreateBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task DeleteBookingAsync(int id);
        Task<IEnumerable<BookingDto>> GetBookingsByUserIdAsync(int userId);
        Task<IEnumerable<BookingDto>> GetBookingsByResourceIdAsync(int resourceId);
    }
}
