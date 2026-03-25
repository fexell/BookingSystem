using BookingSystem.Api.Models;
using BookingSystem.Shared.DTOs;

namespace BookingSystem.Api.Helpers;

public static class UserMapperHelper {
    public static UserResponse ToResponse( User u ) => new(
        u.Id,
        u.FirstName,
        u.Surname,
        u.UserName,
        u.Email
    );
}
