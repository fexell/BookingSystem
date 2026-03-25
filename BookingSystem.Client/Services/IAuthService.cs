using BookingSystem.Shared.DTOs;

namespace BookingSystem.Client.Services;

public interface IAuthService {
    Task<(bool Success, string? Error)> RegisterAsync( RegisterRequest request );
    Task<bool> LoginAsync( string email, string password );
    Task LogoutAsync();
}
