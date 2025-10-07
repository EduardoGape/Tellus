using Xunit;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellusAPI.Application.Services;
using TellusAPI.Domain.Entities;
using TellusAPI.Application.DTOs;
using TellusAPI.Application.Interfaces;

namespace TellusAPI.UnitTests.Services
{
    public class ProfileServiceTests
    {
        private readonly Mock<IDatabaseExecutor> _dbMock;
        private readonly ProfileService _service;

        public ProfileServiceTests()
        {
            _dbMock = new Mock<IDatabaseExecutor>();
            _service = new ProfileService(_dbMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfProfiles()
        {
            var expected = new List<Profile>
            {
                new() { Id = 1, Name = "Admin", Active = true, Functions = new List<Function>() },
                new() { Id = 2, Name = "Guest", Active = false, Functions = new List<Function>() }
            };

            _dbMock.Setup(db => db.QueryAsync<Profile>(It.IsAny<string>(), null, null))
                   .ReturnsAsync(expected);

            var result = await _service.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Name == "Admin");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProfile_WhenExists()
        {
            var expected = new Profile { Id = 1, Name = "Admin", Active = true, Functions = new List<Function>() };

            _dbMock.Setup(db => db.QueryFirstOrDefaultAsync<Profile>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(expected);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Admin");
            result.Active.Should().BeTrue();
            result.Functions.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedProfile()
        {
            var dto = new CreateProfileDto
            {
                Name = "Manager",
                Active = true,
                Functions = new List<Function>()
            };

            var expected = new Profile
            {
                Id = 3,
                Name = dto.Name,
                Active = dto.Active,
                Functions = dto.Functions
            };

            _dbMock.Setup(db => db.QueryFirstAsync<Profile>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(expected);

            var result = await _service.CreateAsync(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Manager");
            result.Active.Should().BeTrue();
            result.Id.Should().Be(3);
            result.Functions.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedProfile_WhenExists()
        {
            var dto = new UpdateProfileDto
            {
                Name = "SuperAdmin",
                Active = true,
                Functions = new List<Function>()
            };

            var expected = new Profile
            {
                Id = 1,
                Name = dto.Name,
                Active = dto.Active,
                Functions = dto.Functions
            };

            _dbMock.Setup(db => db.QueryFirstOrDefaultAsync<Profile>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(expected);

            var result = await _service.UpdateAsync(1, dto);

            result.Should().NotBeNull();
            result!.Name.Should().Be("SuperAdmin");
            result.Active.Should().BeTrue();
            result.Functions.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenAffectedRowsGreaterThanZero()
        {
            _dbMock.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(1);

            var result = await _service.DeleteAsync(2);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNoRowsAffected()
        {
            _dbMock.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(0);

            var result = await _service.DeleteAsync(2);

            result.Should().BeFalse();
        }
    }
}
