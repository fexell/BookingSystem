using BookingSystem.Api.Models;

namespace BookingSystem.Api.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(string username, string email, string password);
        Task<User?> LoginAsync(string email, string password);
        string GenerateToken( User user );
        Task<string> GenerateRefreshTokenAsync( User user );
        Task<User?> RefreshAsync(string refreshToken);
        Task RevokeRefreshTokensAsync( int userId );
    }
}