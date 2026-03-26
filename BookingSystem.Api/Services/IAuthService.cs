using BookingSystem.Api.Models;

namespace BookingSystem.Api.Services;

public interface IAuthService {
    Task<User> RegisterAsync( string username, string firstName, string surname, string email, string password );
    Task<User?> LoginAsync( string email, string password );
    Task<string> GenerateRefreshTokenAsync( User user );
    Task<User?> RefreshAsync( string refreshToken );
    Task<string> GenerateTokenAsync( User user );
    Task RevokeRefreshTokenAsync ( string refreshToken );
    Task RevokeAllSessionsAsync ( int userId );
}