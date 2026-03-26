using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using BookingSystem.Api.Models;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int> {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data för Resources
        modelBuilder.Entity<Resource>().HasData(
        new Resource { Id = 1, Name = "Studio A", Description = "Big recording studio with drums, guitar amps, and a vocal booth.", Type = "Studio", IsAvailable = true },
        new Resource { Id = 2, Name = "Studio B", Description = "Smaller studio, perfect for vocals and acoustic instruments.", Type = "Studio", IsAvailable = true },
        new Resource { Id = 3, Name = "Guitar Amp", Description = "Fender Twin Reverb, great for clean tones and classic rock.", Type = "Equipment", IsAvailable = true },
        new Resource { Id = 4, Name = "Vocal Booth", Description = "Soundproof booth with a condenser microphone and pop filter.", Type = "Equipment", IsAvailable = true },
        new Resource { Id = 5, Name = "Control Room", Description = "Room with mixing console, monitors, and recording software.", Type = "Studio", IsAvailable = true }
    );


        //Roller
        modelBuilder.Entity<IdentityRole<int>>().HasData(
        new IdentityRole<int> 
        { 
            Id = 1,
            Name = "Admin",
            NormalizedName = "ADMIN", 
            ConcurrencyStamp = "role-admin-stamp-001" //används av EF Core för att undvika konflikter vid samtidiga uppdateringar
        },
        new IdentityRole<int> 
        { 
            Id = 2,
            Name = "User", 
            NormalizedName = "USER", 
            ConcurrencyStamp = "role-admin-stamp-002" 
        }
    );


        //Users

        var admin = new User
        {
            Id = 1,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            FirstName = "Peter",
            Surname = "Andersson",
            Email = "admin@studio.se",
            NormalizedEmail = "ADMIN@STUDIO.SE",
            EmailConfirmed = true,
            SecurityStamp = "admin-security-stamp-001", //används av Identity för att invalidera tokens vid lösenordsbyte
            ConcurrencyStamp = "admin-concurrency-stamp-001",
            PasswordHash = "AQAAAAIAAYagAAAAEFwcWzGVTcou6m4l3QfSB1EXixnj2xMRgwNCPUyCPdXWB7FxPFlwxCeszH1WVlxU+A=="
        };

        var user1 = new User
        {
            Id = 2,
            UserName = "anna",
            NormalizedUserName = "ANNA",
            FirstName = "Anna",
            Surname = "Svensson",
            Email = "anna@user.com",
            NormalizedEmail = "ANNA@USER.COM",
            EmailConfirmed = true,
            SecurityStamp = "anna-security-stamp-002",
            ConcurrencyStamp = "anna-concurrency-stamp-002",
            PasswordHash = "AQAAAAIAAYagAAAAEI/PR+O8rGS5fyDzwjlV8EljBATSnF2r3ewX9LhFkrMR4FBn1zAWU+1Yvz+uPsWZlg=="
        };

        var user2 = new User
        {
            Id = 3,
            UserName = "erik",
            NormalizedUserName = "ERIK",
            FirstName = "Erik",
            Surname = "Johansson",
            Email = "erik@example.com",
            NormalizedEmail = "ERIK@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = "erik-security-stamp-003",
            ConcurrencyStamp = "erik-concurrency-stamp-003",
            PasswordHash = "AQAAAAIAAYagAAAAENKjH98XL+b364+OivWCDlVt9xdw/rUR4JIT9geVokYZWpybdfuKp71pqEIJyPUOrg=="
        };

        modelBuilder.Entity<User>().HasData(admin, user1, user2);

        // UserRoles
        modelBuilder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int> { UserId = 1, RoleId = 1 }, // Admin
            new IdentityUserRole<int> { UserId = 2, RoleId = 2 }, // User
            new IdentityUserRole<int> { UserId = 3, RoleId = 2 }  // User
        );

        // Seed data för Bookings
        modelBuilder.Entity<Booking>().HasData(
            new Booking
            {
                Id = 1,
                UserId = 2,
                ResourceId = 1,
                FirstName = "Anna",
                Surname = "Svensson",
                StartTime = new DateTime(2026, 3, 27, 10, 0, 0),
                EndTime = new DateTime(2026, 3, 27, 12, 0, 0),
                Status = "Active",
                PartySize = 2,
                Notes = "Need drumset",
                CreatedAt = new DateTime(2026, 3, 20, 10, 0, 0)
            },
             new Booking
    {
               Id = 2,
               UserId = 3,
               ResourceId = 2,
               FirstName = "Erik",
               Surname = "Johansson",
               StartTime = new DateTime(2026, 3, 27, 13, 0, 0),
               EndTime = new DateTime(2026, 3, 27, 15, 0, 0),
               Status = "Active",
               PartySize = 1,
               Notes = "Acoustic session",
               CreatedAt = new DateTime(2026, 3, 20, 11, 0, 0)
             }
        );
    }
}