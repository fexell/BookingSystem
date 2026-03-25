

using BookingSystem.Api.Models;

namespace BookingSystem.Api.Repositories;

public interface IRefreshTokenRepository {
    Task<RefreshToken?> GetByTokenAsync( string token );
    Task AddAsync( RefreshToken refreshToken );
    Task RevokeAsync( string refreshToken );
    Task RevokeAllForUserAsync( int userId );
    Task UpdateAsync( RefreshToken refreshToken );
}