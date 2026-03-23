using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;

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

        public async Task<User> RegisterAsync( string username, string email, string password ) {
            var normalizedEmail = email.Trim().ToLower();
            var normalizedUsername = username.Trim().ToLower();

            var user = new User {
                Name = normalizedUsername,
                UserName = normalizedEmail,
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

        public string GenerateToken( User user ) {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes( _configuration[ "Jwt:Key" ]! ) );
            var credentials = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, "User")
            };
            var token = new JwtSecurityToken(
                issuer: _configuration[ "Jwt:Issuer" ],
                audience: _configuration[ "Jwt:Audience" ],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse( _configuration[ "Jwt:AccessTokenLifetime" ]! ) ),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken( token );
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

        public async Task<User?> RefreshAsync( string refreshToken ) {
            var stored = await _refreshTokenRepository.GetByTokenAsync( refreshToken );
            if ( stored == null ) return null;
            if ( stored.IsRevoked ) return null;
            if ( stored.Expiry < DateTime.UtcNow ) return null;

            stored.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync( stored );

            return await _userManager.FindByIdAsync( stored.UserId.ToString() );
        }

        public async Task RevokeRefreshTokensAsync( int userId ) {
            await _refreshTokenRepository.RevokeAllForUserAsync( userId );
        }
    }
}