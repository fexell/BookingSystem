using BookingSystem.Api.Models;

namespace BookingSystem.Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task UpdateUserRoleAsync( User user, string newRole );
    }
}