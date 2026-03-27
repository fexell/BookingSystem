using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;
using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        public async Task UpdateUserRoleAsync( User user, string newRole ) {
            var currentRoles = await _userManager.GetRolesAsync( user );

            // Remove old roles
            if ( currentRoles.Any() )
                await _userManager.RemoveFromRolesAsync( user, currentRoles );

            // Add new role
            await _userManager.AddToRoleAsync( user, newRole );
        }
    }
}