using Microsoft.AspNetCore.Identity;

namespace BookingSystem.Api.Models;

public class User : IdentityUser<int> {

    public string Name { get; set; } = string.Empty;
}