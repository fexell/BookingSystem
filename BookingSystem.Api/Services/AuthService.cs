using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;
using BookingSystem.Api.Helpers;

namespace BookingSystem.Api.Services {
    public class AuthService : IAuthService {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            UserManager<User> userManager,
            IConfiguration configuration,
            IRefreshTokenRepository refreshTokenRepository ) {
            _userManager = userManager;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<User> RegisterAsync( string username, string firstName, string surname, string email, string password ) {
            var normalizedEmail = email.Trim().ToLower();
            var normalizedUsername = username.Trim().ToLower();
            var normalizedFirstName = char.ToUpper( firstName[ 0 ] ) + firstName.Substring( 1 ).ToLower();
            var normalizedSurname = char.ToUpper( surname[ 0 ] ) + surname.Substring( 1 ).ToLower();

            var user = new User {
                FirstName = normalizedFirstName,
                Surname = normalizedSurname,
                UserName = normalizedUsername,
                Email = normalizedEmail
            };

            // Identity handles duplicate checks, password hashing, and validation
            var result = await _userManager.CreateAsync( user, password );
            if ( !result.Succeeded )
                throw new InvalidOperationException( result.Errors.First().Description );

            await _userManager.AddToRoleAsync( user, "User" );
            return user;
        }

        public async Task<User?> LoginAsync( string email, string password ) {
            var normalizedEmail = email.Trim().ToLower();
            var user = await _userManager.FindByEmailAsync( normalizedEmail );
            if ( user == null ) return null;

            // Identity handles lockout and password verification
            var result = await _userManager.CheckPasswordAsync( user, password );
            if ( !result ) return null;

            return user;
        }

        public async Task<string> GenerateRefreshTokenAsync( User user ) {
            var randomBytes = new byte[ 64 ];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes( randomBytes );
            var token = Convert.ToBase64String( randomBytes );

            await _refreshTokenRepository.AddAsync( new RefreshToken {
                Token = token,
                Expiry = DateTime.UtcNow.AddDays(
                    int.Parse( _configuration[ "Jwt:RefreshTokenLifetime" ]! ) ),
                UserId = user.Id
            } );

            return token;
        }

        public string GenerateToken( User user ) => TokenHelper.GenerateToken( user, _configuration );

        public async Task<User?> RefreshAsync ( string refreshToken ) {
            var stored = await _refreshTokenRepository.GetByTokenAsync( refreshToken );
            if ( stored is null || stored.IsRevoked || stored.Expiry < DateTime.UtcNow )
                return null;

            await _refreshTokenRepository.RevokeAsync( refreshToken );
            return await _userManager.FindByIdAsync( stored.UserId.ToString() );
        }

        public async Task RevokeAllSessionsAsync( int userId ) {
            await _refreshTokenRepository.RevokeAllForUserAsync( userId );
        }

        public async Task RevokeRefreshTokenAsync( string refreshToken ) {
            await _refreshTokenRepository.RevokeAsync( refreshToken );
        }
    }
}