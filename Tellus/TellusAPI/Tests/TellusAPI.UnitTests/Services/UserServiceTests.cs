using Xunit;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TellusAPI.Application.Services;
using TellusAPI.Domain.Entities;
using TellusAPI.Domain.Entities.Reference;
using TellusAPI.Application.DTOs;
using TellusAPI.Application.Interfaces;
using TellusAPI.Application.Filters;
using TellusAPI.Application.Common;
using Microsoft.Extensions.Configuration;

namespace TellusAPI.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IDatabaseExecutor> _dbMock;
        private readonly UserService _service;
        private readonly IConfiguration _configuration;

        public UserServiceTests()
        {
            _dbMock = new Mock<IDatabaseExecutor>();

            var inMemorySettings = new Dictionary<string, string>
            {
                {"JWT:SecretKey", "0ae1c7c01a714cd9b45134ac180ead05b3036572b97a4878a6ea571006c2e929"}
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _service = new UserService(_dbMock.Object, _configuration);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfUsers()
        {
            var expected = new List<User>
            {
                new() { Id = 1, Name = "Alice", Email = "alice@test.com", Password = "pass123", Profile = new ProfileReference { Id = 1, Name = "Admin" } },
                new() { Id = 2, Name = "Bob", Email = "bob@test.com", Password = "pass456", Profile = new ProfileReference { Id = 2, Name = "Guest" } }
            };

            _dbMock.Setup(db => db.QueryAsync<User>(It.IsAny<string>(), null, null))
                   .ReturnsAsync(expected);

            var result = await _service.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(u => u.Name == "Alice");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
        {
            var expected = new User
            {
                Id = 1,
                Name = "Alice",
                Email = "alice@test.com",
                Password = "pass123",
                Profile = new ProfileReference { Id = 1, Name = "Admin" }
            };

            _dbMock.Setup(db => db.QueryFirstOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(expected);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Alice");
            result.Email.Should().Be("alice@test.com");
            result.Profile.Name.Should().Be("Admin");
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedUser()
        {
            var dto = new CreateUserDto
            {
                Name = "Charlie",
                Email = "charlie@test.com",
                Password = "pass789",
                Profile = new ProfileReference { Id = 1, Name = "Admin" }
            };

            var expected = new User
            {
                Id = 3,
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Profile = dto.Profile
            };

            _dbMock.Setup(db => db.QueryFirstAsync<User>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(expected);

            var result = await _service.CreateAsync(dto);

            result.Should().NotBeNull();
            result.Id.Should().Be(3);
            result.Name.Should().Be("Charlie");
            result.Email.Should().Be("charlie@test.com");
            result.Profile.Name.Should().Be("Admin");
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedUser_WhenExists()
        {
            var dto = new UpdateUserDto
            {
                Name = "AliceUpdated",
                Email = "aliceupdated@test.com",
                Password = "newpass",
                Profile = new ProfileReference { Id = 2, Name = "Guest" }
            };

            var expected = new User
            {
                Id = 1,
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Profile = dto.Profile
            };

            _dbMock.Setup(db => db.QueryFirstOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(expected);

            var result = await _service.UpdateAsync(1, dto);

            result.Should().NotBeNull();
            result!.Name.Should().Be("AliceUpdated");
            result.Email.Should().Be("aliceupdated@test.com");
            result.Profile.Name.Should().Be("Guest");
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

        [Fact]
        public async Task SearchAsync_ShouldReturnPagedResult()
        {
            var filter = new UserFilter
            {
                Name = "Alice",
                Page = 1,
                PageSize = 10
            };

            var expectedUsers = new List<User>
            {
                new User { Id = 1, Name = "Alice", Email = "alice@test.com", Password = "pass123", Profile = new ProfileReference { Id = 1, Name = "Admin" } }
            };

            _dbMock.Setup(db => db.QueryFirstAsync<int>(It.Is<string>(s => s.StartsWith("SELECT COUNT")), It.IsAny<object>(), null))
                   .ReturnsAsync(expectedUsers.Count);

            _dbMock.Setup(db => db.QueryAsync<User>(It.Is<string>(s => s.StartsWith("SELECT *")), It.IsAny<object>(), null))
                   .ReturnsAsync(expectedUsers);

            var result = await _service.SearchAsync(filter);

            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Total.Should().Be(1);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.Items[0].Name.Should().Be("Alice");
        }

        // ===== Testes para LoginAsync =====
        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var user = new User
            {
                Id = 1,
                Email = "alice@test.com",
                Password = "pass123",
                Name = "Alice"
            };

            _dbMock.Setup(db => db.QueryFirstOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync(user);

            var token = await _service.LoginAsync("alice@test.com", "pass123");

            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            _dbMock.Setup(db => db.QueryFirstOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>(), null))
                   .ReturnsAsync((User?)null);

            var token = await _service.LoginAsync("alice@test.com", "wrongpass");

            token.Should().BeNull();
        }
    }
}
