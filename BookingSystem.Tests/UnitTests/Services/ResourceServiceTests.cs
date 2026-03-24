using System.Collections.Generic;
using System.Threading.Tasks;
using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;
using BookingSystem.Api.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookingSystem.Tests.UnitTests.Services
{
    public class ResourceServiceTests
    {
        private readonly Mock<IResourceRepository> _mockResourceRepository;
        private readonly ResourceService _resourceService;

        public ResourceServiceTests()
        {
            _mockResourceRepository = new Mock<IResourceRepository>();
            _resourceService = new ResourceService(_mockResourceRepository.Object);
        }

        [Fact]
        public async Task GetResourceByIdAsync_ShouldReturnNull_WhenResourceDoesNotExist()
        {
            // Arrange
            _mockResourceRepository
                .Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Resource)null!);

            // Act
            var result = await _resourceService.GetResourceByIdAsync(999);

            // Assert
            result.Should().BeNull();
            _mockResourceRepository.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task CreateResourceAsync_ShouldReturnResource_WhenCalled()
        {
            // Arrange
            var resource = new Resource { Id = 1, Name = "Meeting Room A", IsAvailable = true };

            // Act
            var result = await _resourceService.CreateResourceAsync(resource);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Meeting Room A");
            _mockResourceRepository.Verify(repo => repo.AddAsync(resource), Times.Once);
        }

        [Fact]
        public async Task GetAvailableResourcesAsync_ShouldReturnOnlyAvailableResources()
        {
            // Arrange
            var resources = new List<Resource>
            {
                new Resource { Id = 1, Name = "Available Room", IsAvailable = true },
                new Resource { Id = 2, Name = "Available Room 2", IsAvailable = true }
            };

            _mockResourceRepository
                .Setup(repo => repo.GetAvailableResourcesAsync())
                .ReturnsAsync(resources);

            // Act
            var result = await _resourceService.GetAvailableResourcesAsync();

            // Assert
            result.Should().HaveCount(2);
        }
    }
}
