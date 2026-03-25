using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Api.Models;

public class User : IdentityUser<int> {

    public string? FirstName { get; set; }
    public string? Surname { get; set; }
}