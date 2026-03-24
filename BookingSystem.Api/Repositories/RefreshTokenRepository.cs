
using Microsoft.EntityFrameworkCore;

using BookingSystem.Api.Models;

namespace BookingSystem.Api.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository {
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context) {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token) {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync( rt => rt.Token == token );
    }

    public async Task AddAsync(RefreshToken refreshToken) {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAsync ( string refreshToken ) {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync( rt => rt.Token == refreshToken );
        if ( token is null ) return;
        token.IsRevoked = true;
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllForUserAsync( int userId ) {
        await _context.RefreshTokens
            .Where( rt => rt.UserId == userId && !rt.IsRevoked )
            .ExecuteUpdateAsync( s => s.SetProperty( rt => rt.IsRevoked, true ) );
    }

    public async Task UpdateAsync(RefreshToken refreshToken) {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
}