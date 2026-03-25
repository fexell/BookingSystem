using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;
using BookingSystem.Api.Services;
using BookingSystem.Shared.DTOs;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookingSystem.Tests.UnitTests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<IResourceRepository> _mockResourceRepository;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockResourceRepository = new Mock<IResourceRepository>();
            _bookingService = new BookingService(_mockBookingRepository.Object, _mockResourceRepository.Object);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldAddBooking_WhenNoOverlapExists()
        {
            // Arrange
            var newBooking = new Booking
            {
                ResourceId = 1,
                UserId = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                EndTime = new DateTime(2026, 1, 1, 12, 0, 0),
                Status = "Active"
            };

            _mockBookingRepository
                .Setup(repo => repo.GetByResourceIdAsync(newBooking.ResourceId))
                .ReturnsAsync(new List<Booking>()); // Inga befintliga bokningar

            // Act
            var result = await _bookingService.CreateBookingAsync(newBooking);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BookingDto>();
            result.ResourceId.Should().Be(1);
            _mockBookingRepository.Verify(repo => repo.AddAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowInvalidOperationException_WhenBookingOverlaps()
        {
            // Arrange
            var existingBooking = new Booking
            {
                Id = 1,
                ResourceId = 1,
                UserId = 2,
                StartTime = new DateTime(2026, 1, 1, 9, 0, 0),
                EndTime = new DateTime(2026, 1, 1, 11, 0, 0), // Överlappar med den nya bokningen
                Status = "Active"
            };

            var newBooking = new Booking
            {
                ResourceId = 1,
                UserId = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0), // Börjar innan existingBooking är slut
                EndTime = new DateTime(2026, 1, 1, 12, 0, 0),
                Status = "Active"
            };

            _mockBookingRepository
                .Setup(repo => repo.GetByResourceIdAsync(newBooking.ResourceId))
                .ReturnsAsync(new List<Booking> { existingBooking });

            // Act
            Func<Task> action = async () => await _bookingService.CreateBookingAsync(newBooking);

            // Assert
            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Resource is already booked during this time.");
            
            _mockBookingRepository.Verify(repo => repo.AddAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldNotThrow_WhenExistingBookingIsCancelled()
        {
            // Arrange
            var cancelledBooking = new Booking
            {
                Id = 1,
                ResourceId = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                EndTime = new DateTime(2026, 1, 1, 12, 0, 0),
                Status = "Cancelled" // Inte "Active", vi förväntar oss att denna ignoreras
            };

            var newBooking = new Booking
            {
                ResourceId = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                EndTime = new DateTime(2026, 1, 1, 12, 0, 0),
                Status = "Active"
            };

            _mockBookingRepository
                .Setup(repo => repo.GetByResourceIdAsync(newBooking.ResourceId))
                .ReturnsAsync(new List<Booking> { cancelledBooking });

            // Act
            var result = await _bookingService.CreateBookingAsync(newBooking);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BookingDto>();
            _mockBookingRepository.Verify(repo => repo.AddAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldNotThrow_WhenBookingsAreAdjacent()
        {
            // Arrange
            var existingBooking = new Booking
            {
                Id = 1,
                ResourceId = 1,
                StartTime = new DateTime(2026, 1, 1, 8, 0, 0),
                EndTime = new DateTime(2026, 1, 1, 10, 0, 0),
                Status = "Active"
            };

            var newBooking = new Booking
            {
                ResourceId = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0), // Börjar exakt när den förra slutar
                EndTime = new DateTime(2026, 1, 1, 12, 0, 0),
                Status = "Active"
            };

            _mockBookingRepository
                .Setup(repo => repo.GetByResourceIdAsync(newBooking.ResourceId))
                .ReturnsAsync(new List<Booking> { existingBooking });

            // Act
            var result = await _bookingService.CreateBookingAsync(newBooking);

            // Assert
            result.Should().NotBeNull();
            _mockBookingRepository.Verify(repo => repo.AddAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task GetAllBookingsAsync_ShouldReturnAllBookings()
        {
            // Arrange
            var bookingsList = new List<Booking>
            {
                new Booking { Id = 1, ResourceId = 1 },
                new Booking { Id = 2, ResourceId = 2 }
            };

            _mockBookingRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(bookingsList);

            // Act
            var result = await _bookingService.GetAllBookingsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(b => b.Id == 1);
            result.First().Should().BeOfType<BookingDto>();
        }
    }
}
