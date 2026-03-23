using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using BookingSystem.Api.Models;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int> {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}