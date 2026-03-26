using BookingSystem.Shared.DTOs;

namespace BookingSystem.Client.Services;

public interface IBookingService {
    Task<bool> CreateBookingAsync( CreateBookingRequest request );
}