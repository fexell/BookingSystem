using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Api.Controllers;
using BookingSystem.Api.Models;
using BookingSystem.Api.Services;
using BookingSystem.Shared.DTOs;
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
        public async Task GetAll_ShouldReturnOk_WithBookings()
        {
            // Arrange
            var bookings = new List<BookingDto> { new BookingDto { Id = 1 } };
            _mockBookingService.Setup(s => s.GetAllBookingsAsync()).ReturnsAsync(bookings);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var returnedBookings = okResult!.Value as IEnumerable<BookingDto>;
            returnedBookings.Should().NotBeNull();
            returnedBookings!.Any(b => b.Id == 1).Should().BeTrue();
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            // Arrange
            _mockBookingService
                .Setup(s => s.GetBookingByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((BookingDto)null!);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenBookingExists()
        {
            // Arrange
            var booking = new BookingDto { Id = 1, ResourceId = 1 };
            _mockBookingService
                .Setup(s => s.GetBookingByIdAsync(1))
                .ReturnsAsync(booking);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var returnedBooking = okResult!.Value as BookingDto;
            returnedBooking.Should().NotBeNull();
            returnedBooking!.Id.Should().Be(1);
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
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = result.Result as BadRequestObjectResult;
            badRequest!.Value.Should().Be("Resource is already booked during this time.");
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenSuccessful()
        {
            // Arrange
            var booking = new Booking { Id = 1, ResourceId = 1 };
            var returnedDto = new BookingDto { Id = 1, ResourceId = 1 };
            _mockBookingService.Setup(s => s.CreateBookingAsync(booking)).ReturnsAsync(returnedDto);

            // Act
            var result = await _controller.Create(booking);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedBooking = createdResult!.Value as BookingDto;
            returnedBooking.Should().NotBeNull();
            returnedBooking!.Id.Should().Be(1);
        }
    }
}
