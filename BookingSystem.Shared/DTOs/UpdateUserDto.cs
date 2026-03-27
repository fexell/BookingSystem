namespace BookingSystem.Shared.DTOs;

public class UpdateUserDto {
    public string Email { get; set; } = "";
    public string UserName { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string Surname { get; set; } = "";
    public string Role { get; set; } = "";
}
