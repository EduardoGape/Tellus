using Xunit;
using Dapper;
using FluentAssertions;
using Npgsql;
using System.Threading.Tasks;
using TellusAPI.Application.Services;
using TellusAPI.Domain.Entities.Reference;
using TellusAPI.Application.DTOs;
using TellusAPI.Application.Interfaces;
using TellusAPI.Application.Filters;
using TellusAPI.Application.Common;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace TellusAPI.IntegrationTests.Services
{
    public class UserServiceIntegrationTests : IAsyncLifetime
    {
        private static bool _handlersRegistered = false;

        private NpgsqlConnection _dbConnection = null!;
        private IDatabaseExecutor _dbExecutor = null!;
        private UserService _service = null!;
        private IConfiguration _configuration = null!;
        private readonly string _connectionString;

        public UserServiceIntegrationTests()
        {
            _connectionString = "Host=localhost;Database=tellus_test;Username=postgres;Password=postgres;Port=5432";

            if (!_handlersRegistered)
            {
                SqlMapper.AddTypeHandler(new JsonTypeHandler<ProfileReference>());
                _handlersRegistered = true;
            }

            var inMemorySettings = new Dictionary<string, string>
            {
                {"JWT:SecretKey", "0ae1c7c01a714cd9b45134ac180ead05b3036572b97a4878a6ea571006c2e929"}
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        public async Task InitializeAsync()
        {
            _dbConnection = new NpgsqlConnection(_connectionString);
            await _dbConnection.OpenAsync();

            _dbExecutor = new DapperDatabaseExecutor(_dbConnection);
            _service = new UserService(_dbExecutor, _configuration);

            await CleanDatabaseAsync();
            await SeedUsersAsync();
        }

        public Task DisposeAsync()
        {
            _dbConnection.Close();
            return Task.CompletedTask;
        }

        private async Task CleanDatabaseAsync()
        {
            const string sql = @"
                TRUNCATE TABLE users RESTART IDENTITY;
                ALTER SEQUENCE users_id_seq RESTART WITH 1;";
            await _dbConnection.ExecuteAsync(sql);
        }

        private async Task SeedUsersAsync()
        {
            const string sql = @"
                INSERT INTO users (name, email, password, profile)
                VALUES 
                    ('Alice', 'alice@test.com', 'pass123', '{""Id"":1,""Name"":""Admin""}'::jsonb),
                    ('Bob', 'bob@test.com', 'pass456', '{""Id"":2,""Name"":""Guest""}'::jsonb),
                    ('Charlie', 'charlie@test.com', 'pass789', '{""Id"":1,""Name"":""Admin""}'::jsonb);";
            await _dbConnection.ExecuteAsync(sql);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnSeededUsers()
        {
            var result = await _service.GetAllAsync();
            result.Should().HaveCount(3);
            result.Should().Contain(u => u.Name == "Alice");
            result.Should().Contain(u => u.Name == "Bob");
            result.Should().Contain(u => u.Name == "Charlie");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSpecificUser()
        {
            var result = await _service.GetByIdAsync(1);
            result.Should().NotBeNull();
            result!.Name.Should().Be("Alice");
            result.Email.Should().Be("alice@test.com");
            result.Profile.Name.Should().Be("Admin");
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertAndReturnNewUser()
        {
            var dto = new CreateUserDto
            {
                Name = "David",
                Email = "david@test.com",
                Password = "pass999",
                Profile = new ProfileReference { Id = 2, Name = "Guest" }
            };

            var created = await _service.CreateAsync(dto);
            created.Should().NotBeNull();
            created.Name.Should().Be("David");
            created.Email.Should().Be("david@test.com");
            created.Profile.Name.Should().Be("Guest");

            var all = await _service.GetAllAsync();
            all.Should().Contain(u => u.Name == "David");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingUser()
        {
            var dto = new UpdateUserDto
            {
                Name = "AliceUpdated",
                Email = "aliceupdated@test.com",
                Password = "newpass",
                Profile = new ProfileReference { Id = 2, Name = "Guest" }
            };

            var updated = await _service.UpdateAsync(1, dto);
            updated.Should().NotBeNull();
            updated!.Name.Should().Be("AliceUpdated");
            updated.Email.Should().Be("aliceupdated@test.com");
            updated.Profile.Name.Should().Be("Guest");

            var reloaded = await _service.GetByIdAsync(1);
            reloaded!.Name.Should().Be("AliceUpdated");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveUser()
        {
            var deleted = await _service.DeleteAsync(2);
            deleted.Should().BeTrue();

            var result = await _service.GetAllAsync();
            result.Should().HaveCount(2);
            result.Should().NotContain(u => u.Id == 2);
        }

        [Fact]
        public async Task SearchAsync_ShouldFilterByName()
        {
            var filter = new UserFilter { Name = "Alice" };
            var result = await _service.SearchAsync(filter);

            result.Items.Should().ContainSingle(u => u.Name == "Alice");
            result.Total.Should().Be(1);
        }

        [Fact]
        public async Task SearchAsync_ShouldFilterByEmail()
        {
            var filter = new UserFilter { Email = "bob@test.com" };
            var result = await _service.SearchAsync(filter);

            result.Items.Should().ContainSingle(u => u.Email == "bob@test.com");
            result.Total.Should().Be(1);
        }

        [Fact]
        public async Task SearchAsync_ShouldFilterByProfileId()
        {
            var filter = new UserFilter { ProfileId = 1 };
            var result = await _service.SearchAsync(filter);

            result.Items.Should().HaveCount(2);
            result.Items.Should().AllSatisfy(u => u.Profile.Id.Should().Be(1));
            result.Total.Should().Be(2);
        }

        [Fact]
        public async Task SearchAsync_ShouldPaginateResults()
        {
            var filter = new UserFilter { Page = 2, PageSize = 2 };
            var result = await _service.SearchAsync(filter);

            result.Page.Should().Be(2);
            result.PageSize.Should().Be(2);
            result.Items.Should().NotBeEmpty();
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var token = await _service.LoginAsync("alice@test.com", "pass123");
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            var token = await _service.LoginAsync("alice@test.com", "wrongpass");
            token.Should().BeNull();
        }
    }
}
