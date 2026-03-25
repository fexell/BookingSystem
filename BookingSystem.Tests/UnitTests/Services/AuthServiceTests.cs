using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;
using BookingSystem.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BookingSystem.Tests.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Getting a Mock of UserManager is slightly more involved because it doesn't have an empty parameterless constructor
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            
            _mockConfiguration = new Mock<IConfiguration>();
            _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();

            _authService = new AuthService(
                _mockUserManager.Object,
                _mockConfiguration.Object,
                _mockRefreshTokenRepository.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenDetailsAreValid()
        {
            // Arrange
            var username = "TestUser";
            var email = "test@example.com";
            var password = "StrongPassword123!";

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<User>(), password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager
                .Setup(m => m.AddToRoleAsync(It.IsAny<User>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var user = await _authService.RegisterAsync(username, email, password);

            // Assert
            user.Should().NotBeNull();
            user.UserName.Should().Be("testuser"); // It normalizes to lowercase
            user.Email.Should().Be("test@example.com");
            _mockUserManager.Verify(m => m.CreateAsync(It.IsAny<User>(), password), Times.Once);
            _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), "User"), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUserManagerFails()
        {
            // Arrange
            var username = "FailUser";
            var email = "fail@example.com";
            var password = "Weak";

            var failedResult = IdentityResult.Failed(new IdentityError { Description = "Password too weak" });

            _mockUserManager
                .Setup(m => m.CreateAsync(It.IsAny<User>(), password))
                .ReturnsAsync(failedResult);

            // Act
            Func<Task> action = async () => await _authService.RegisterAsync(username, email, password);

            // Assert
            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Password too weak");
            
            _mockUserManager.Verify(m => m.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var email = "test@example.com";
            var password = "StrongPassword123!";
            
            var user = new User { Email = email, UserName = "testuser" };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m.CheckPasswordAsync(user, password))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(email);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var email = "notfound@example.com";
            var password = "Password123!";

            _mockUserManager
                .Setup(m => m.FindByEmailAsync(email))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            result.Should().BeNull();
            _mockUserManager.Verify(m => m.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsInvalid()
        {
            // Arrange
            var email = "test@example.com";
            var password = "WrongPassword!";
            
            var user = new User { Email = email, UserName = "testuser" };

            _mockUserManager
                .Setup(m => m.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m.CheckPasswordAsync(user, password))
                .ReturnsAsync(false); // Wrong password

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            result.Should().BeNull();
        }
    }
}
