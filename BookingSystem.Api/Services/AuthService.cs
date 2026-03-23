using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;

namespace BookingSystem.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<User> RegisterAsync(string username, string email, string password)
        {
            var normalizedEmail = email.Trim().ToLower();
            var normalizedUsername = username.Trim().ToLower();

            if ( !System.Text.RegularExpressions.Regex.IsMatch( normalizedEmail,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$" ) )
                throw new InvalidOperationException( "Invalid email format." );

            if(!System.Text.RegularExpressions.Regex.IsMatch(normalizedUsername,
                @"^[a-z0-9_]+$" ) )
                throw new InvalidOperationException( "Username can only contain letters, numbers and underscores." );

            var existingUser = await _userRepository.GetByEmailAsync( normalizedEmail );
            if ( existingUser != null )
                throw new InvalidOperationException( "This email is not available." );

            var exisitingUsername = await _userRepository.GetByUsernameAsync( normalizedUsername );
            if ( exisitingUsername != null )
                throw new InvalidOperationException( "This username is not available." );

            var user = new User {
                Name = normalizedUsername,
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword( password ),
                Role = "User"
            };

            await _userRepository.AddAsync( user );
            return user;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var normalizedEmail = email.Trim().ToLower();

            // Hämta användaren
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);
            if (user == null)
                return null;

            // Kolla lösenordet
            bool isCorrectPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isCorrectPassword)
                return null;

            // Skapa JWT-token
            return user;
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse( _configuration[ "Jwt:AccessTokenLifetime" ]! )
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(User user) {
            var randomBytes = new byte[ 64 ];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes( randomBytes );
            var token = Convert.ToBase64String( randomBytes );

            await _refreshTokenRepository.AddAsync( new RefreshToken {
                Token = token,
                Expiry = DateTime.UtcNow.AddDays(
                    int.Parse( _configuration[ "Jwt:RefreshTokenLifetime" ]! )
                ),
                UserId = user.Id
            } );

            return token;
        }

        public async Task<User?> RefreshAsync(string refreshToken) {
            var stored = await _refreshTokenRepository.GetByTokenAsync( refreshToken );

            if ( stored == null )
                return null;

            if ( stored.IsRevoked )
                return null;

            if ( stored.Expiry < DateTime.UtcNow )
                return null;

            stored.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync( stored );

            return await _userRepository.GetByIdAsync( stored.UserId );
        }

        public async Task RevokeRefreshTokensAsync( int userId ) {
            await _refreshTokenRepository.RevokeAllForUserAsync( userId );
        }
    }
}