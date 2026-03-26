using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BookingSystem.Api.Models;
using BookingSystem.Api.Services;
using BookingSystem.Tests.IntegrationTests.Helpers;
using BookingSystem.Shared.DTOs;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BookingSystem.Tests.IntegrationTests.Controllers
{
    public class BookingsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public BookingsControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private async Task<string> GetJwtTokenAsync(User testUser)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

            // Ensure user exists in our in-memory db
            var dbUser = await db.Users.FindAsync(testUser.Id);
            if (dbUser == null)
            {
                db.Users.Add(testUser);
                await db.SaveChangesAsync();
            }

            return await authService.GenerateTokenAsync(testUser);
        }

        [Fact]
        public async Task GetAll_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/bookings");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithBookings_WhenAuthenticated()
        {
            // Arrange
            var client = _factory.CreateClient();
            var testUser = new User { Id = 1, UserName = "test_user", Email = "test@example.com" };
            
            // Generate a valid JWT token
            var token = await GetJwtTokenAsync(testUser);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Add a mock resource and booking to the In-Memory DB
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                if (await db.Resources.FindAsync(1) == null)
                {
                    db.Resources.Add(new Resource { Id = 1, Name = "Test Room", Type = "Room" });
                }

                db.Bookings.Add(new Booking 
                { 
                    Id = 1, 
                    UserId = testUser.Id, 
                    ResourceId = 1, 
                    Status = "Active",
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddHours(1)
                });
                await db.SaveChangesAsync();
            }

            // Act
            var response = await client.GetAsync("/api/bookings");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedBookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();
            returnedBookings.Should().NotBeNull();
            returnedBookings!.Count.Should().BeGreaterThanOrEqualTo(1);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenBookingOverlaps()
        {
            // Arrange
            var client = _factory.CreateClient();
            var testUser = new User { Id = 2, UserName = "test_user2", Email = "test2@example.com" };
            
            var token = await GetJwtTokenAsync(testUser);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Round to nearest whole hour so the service's minute-validation passes
            var now = DateTime.UtcNow;
            var startTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddDays(1);
            var endTime = startTime.AddHours(2);

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                if (await db.Resources.FindAsync(2) == null)
                {
                    db.Resources.Add(new Resource { Id = 2, Name = "Overlapping Room", Type = "Room" });
                }

                // Existing booking
                db.Bookings.Add(new Booking 
                { 
                    Id = 2, 
                    UserId = testUser.Id, 
                    ResourceId = 2, 
                    Status = "Active",
                    StartTime = startTime,
                    EndTime = endTime 
                });
                await db.SaveChangesAsync();
            }

            // Send a CreateBookingRequest DTO that overlaps with the existing booking
            var requestDto = new CreateBookingRequest(
                StartTime: startTime.AddHours(1),
                EndTime:   startTime.AddHours(3),
                ResourceId: 2
            );

            // Act
            var response = await client.PostAsJsonAsync("/api/bookings", requestDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorText = await response.Content.ReadAsStringAsync();
            errorText.Should().Contain("Studio is already reserved at that time.");
        }
    }
}
