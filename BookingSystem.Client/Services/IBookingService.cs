using BookingSystem.Shared.DTOs;

namespace BookingSystem.Client.Services;

public interface IBookingService {
    Task<List<BookingDto>> GetAllBookingsAsync();
    Task<BookingResult> CreateBookingAsync( CreateBookingRequest request );
    Task<bool> DeleteBookingAsync( int id );
}