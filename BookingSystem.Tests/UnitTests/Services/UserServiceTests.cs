using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;
using BookingSystem.Api.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookingSystem.Tests.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new User { Id = userId, UserName = "john_doe" };
            
            _mockUserRepository
                .Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.UserName.Should().Be("john_doe");
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var email = "jane@example.com";
            var expectedUser = new User { Id = 2, Email = email, UserName = "jane_doe" };
            
            _mockUserRepository
                .Setup(repo => repo.GetByEmailAsync(email))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(email);
        }
    }
}
