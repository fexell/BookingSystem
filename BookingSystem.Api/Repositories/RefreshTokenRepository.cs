

using BookingSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task RevokeAllForUserAsync(int userId) {
        var tokens = await _context.RefreshTokens
            .Where( rt => rt.UserId == userId && !rt.IsRevoked )
            .ToListAsync();

        foreach(var token in tokens) {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken refreshToken) {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
}