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
    public class FunctionServiceTests
    {
        private readonly Mock<IDatabaseExecutor> _dbMock;
        private readonly FunctionService _service;

        public FunctionServiceTests()
        {
            _dbMock = new Mock<IDatabaseExecutor>();
            _service = new FunctionService(_dbMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfFunctions()
        {
            var expected = new List<Function>
            {
                new() { Id = 1, Name = "ReadOnly", CanRead = true, CanWrite = false, IsActive = true },
                new() { Id = 2, Name = "Editor", CanRead = true, CanWrite = true, IsActive = true }
            };

            _dbMock.Setup(db => db.QueryAsync<Function>(It.IsAny<string>(), null, null))
                   .ReturnsAsync(expected);

            var result = await _service.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(f => f.Name == "Editor");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFunction_WhenExists()
        {
            var expected = new Function { Id = 1, Name = "Viewer", CanRead = true, CanWrite = false, IsActive = true };

            _dbMock.Setup(db => db.QueryFirstOrDefaultAsync<Function>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(expected);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Viewer");
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedFunction()
        {
            var dto = new CreateFunctionDto
            {
                Name = "Manager",
                CanRead = true,
                CanWrite = true,
                IsActive = true
            };

            var expected = new Function
            {
                Id = 10,
                Name = dto.Name,
                CanRead = dto.CanRead,
                CanWrite = dto.CanWrite,
                IsActive = dto.IsActive
            };

            _dbMock.Setup(db => db.QueryFirstAsync<Function>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(expected);

            var result = await _service.CreateAsync(dto);

            result.Name.Should().Be("Manager");
            result.Id.Should().Be(10);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedFunction_WhenExists()
        {
            var dto = new UpdateFunctionDto
            {
                Name = "Supervisor",
                CanRead = true,
                CanWrite = false,
                IsActive = true
            };

            var expected = new Function
            {
                Id = 5,
                Name = dto.Name,
                CanRead = dto.CanRead,
                CanWrite = dto.CanWrite,
                IsActive = dto.IsActive
            };

            _dbMock.Setup(db => db.QueryFirstOrDefaultAsync<Function>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(expected);

            var result = await _service.UpdateAsync(5, dto);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Supervisor");
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenAffectedRowsGreaterThanZero()
        {
            _dbMock.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(1);

            var result = await _service.DeleteAsync(3);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNoRowsAffected()
        {
            _dbMock.Setup(db => db.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(0);

            var result = await _service.DeleteAsync(3);

            result.Should().BeFalse();
        }
    }
}
