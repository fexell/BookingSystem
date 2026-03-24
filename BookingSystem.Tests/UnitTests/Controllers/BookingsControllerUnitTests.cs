using System;
using System.Threading.Tasks;
using BookingSystem.Api.Controllers;
using BookingSystem.Api.Models;
using BookingSystem.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookingSystem.Tests.UnitTests.Controllers
{
    public class BookingsControllerUnitTests
    {
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly BookingsController _controller;

        public BookingsControllerUnitTests()
        {
            _mockBookingService = new Mock<IBookingService>();
            _controller = new BookingsController(_mockBookingService.Object);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            _mockBookingService
                .Setup(s => s.GetBookingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Booking)null!);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenBookingExists()
        {
            // Arrange
            var booking = new Booking { Id = 1, ResourceId = 1 };
            _mockBookingService
                .Setup(s => s.GetBookingByIdAsync(1))
                .ReturnsAsync(booking);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(booking);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var booking = new Booking { Id = 2 };

            // Act
            var result = await _controller.Update(1, booking);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().Be("ID does not match.");
            _mockBookingService.Verify(s => s.UpdateBookingAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenServiceThrowsInvalidOperationException()
        {
            // Arrange
            var booking = new Booking { Id = 1, ResourceId = 1 };
            
            _mockBookingService
                .Setup(s => s.CreateBookingAsync(booking))
                .ThrowsAsync(new InvalidOperationException("Resource is already booked during this time."));

            // Act
            var result = await _controller.Create(booking);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest!.Value.Should().Be("Resource is already booked during this time.");
        }
    }
}
