using Xunit;
using Dapper;
using FluentAssertions;
using Npgsql;
using System.Data;
using System.Threading.Tasks;
using TellusAPI.Application.Services;
using TellusAPI.Domain.Entities;
using TellusAPI.Application.DTOs;
using TellusAPI.Application.Interfaces;

namespace TellusAPI.IntegrationTests.Services
{
    public class FunctionServiceIntegrationTests : IAsyncLifetime
    {
        private IDbConnection _dbConnection = null!;
        private IDatabaseExecutor _dbExecutor = null!;
        private FunctionService _service = null!;
        private readonly string _connectionString;

        public FunctionServiceIntegrationTests()
        {
            _connectionString = "Host=localhost;Database=tellus_test;Username=postgres;Password=postgres;Port=5432";
        }

        public async Task InitializeAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            _dbConnection = connection;
            _dbExecutor = new DapperDatabaseExecutor(_dbConnection);
            _service = new FunctionService(_dbExecutor);

            await CleanDatabaseAsync();
            await SeedDataAsync();
        }

        public Task DisposeAsync()
        {
            _dbConnection.Close();
            return Task.CompletedTask;
        }

        private async Task CleanDatabaseAsync()
        {
            var sql = @"
                TRUNCATE TABLE functions RESTART IDENTITY;
                ALTER SEQUENCE functions_id_seq RESTART WITH 1;";
            await _dbConnection.ExecuteAsync(sql);
        }

        private async Task SeedDataAsync()
        {
            var sql = @"
                INSERT INTO functions (name, can_read, can_write, is_active)
                VALUES 
                    ('ReadOnly', true, false, true),
                    ('Editor', true, true, true),
                    ('Disabled', false, false, false)";
            await _dbConnection.ExecuteAsync(sql);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnSeededFunctions()
        {
            var result = await _service.GetAllAsync();

            result.Should().HaveCount(3);
            result.Should().Contain(f => f.Name == "Editor");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSpecificFunction()
        {
            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("ReadOnly");
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertAndReturnNewFunction()
        {
            var dto = new CreateFunctionDto
            {
                Name = "Manager",
                CanRead = true,
                CanWrite = true,
                IsActive = true
            };

            var created = await _service.CreateAsync(dto);

            created.Should().NotBeNull();
            created.Name.Should().Be("Manager");

            var all = await _service.GetAllAsync();
            all.Should().Contain(f => f.Name == "Manager");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingRecord()
        {
            var dto = new UpdateFunctionDto
            {
                Name = "Viewer",
                CanRead = true,
                CanWrite = false,
                IsActive = true
            };

            var updated = await _service.UpdateAsync(2, dto);

            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Viewer");

            var reloaded = await _service.GetByIdAsync(2);
            reloaded!.Name.Should().Be("Viewer");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveRecord()
        {
            var deleted = await _service.DeleteAsync(3);

            deleted.Should().BeTrue();

            var result = await _service.GetAllAsync();
            result.Should().HaveCount(2);
        }
    }
}
