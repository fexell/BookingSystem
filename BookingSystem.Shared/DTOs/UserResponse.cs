namespace BookingSystem.Shared.DTOs;

public record UserResponse(
    int Id,
    string FirstName,
    string Surname,
    string Username,
    string Email,
    string Role
);
