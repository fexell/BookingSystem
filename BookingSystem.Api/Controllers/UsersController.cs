using BookingSystem.Api.Controllers;
using BookingSystem.Api.Helpers;
using BookingSystem.Api.Models;
using BookingSystem.Api.Services;
using BookingSystem.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UsersController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [Authorize( Roles = "Admin" )]
        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var users = await _userService.GetAllUsersAsync();
            var result = new List<UserResponse>();

            foreach ( var user in users ) {
                var roles = await _userManager.GetRolesAsync( user );
                result.Add( UserMapperHelper.ToResponse( user, roles ) );
            }

            return Ok( result );
        }

        [HttpGet( "{id}" )]
        public async Task<IActionResult> GetById( int id ) {
            var requesterId = int.Parse( User.FindFirstValue( ClaimTypes.NameIdentifier )! );

            if ( requesterId != id && !User.IsInRole( "Admin" ) )
                return Forbid();

            var user = await _userService.GetUserByIdAsync( id );
            if ( user == null )
                return NotFound();

            var roles = await _userManager.GetRolesAsync( user );

            return Ok( UserMapperHelper.ToResponse( user, roles ) );
        }

        [HttpGet( "me" )]
        public async Task<IActionResult> GetMe() {
            var userId = int.Parse( User.FindFirstValue( ClaimTypes.NameIdentifier )! );
            var user = await _userService.GetUserByIdAsync( userId );

            if ( user == null )
                return NotFound();

            var roles = await _userManager.GetRolesAsync( user );

            return Ok( UserMapperHelper.ToResponse( user, roles ) );
        }

        [Authorize( Roles = "Admin" )]
        [HttpPut( "{id}" )]
        public async Task<IActionResult> Update( int id, UpdateUserDto dto ) {
            var user = await _userService.GetUserByIdAsync( id );
            if ( user == null )
                return NotFound();

            user.Email = dto.Email;
            user.UserName = dto.UserName;

            await _userService.UpdateUserAsync( user );

            // Rollhantering
            await _userService.UpdateUserRoleAsync( user, dto.Role );

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}