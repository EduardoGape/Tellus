using Xunit;
using Dapper;
using FluentAssertions;
using Npgsql;
using System.Threading.Tasks;
using TellusAPI.Application.Services;
using TellusAPI.Domain.Entities;
using TellusAPI.Application.DTOs;
using TellusAPI.Application.Interfaces;
using System.Collections.Generic;

namespace TellusAPI.IntegrationTests.Services
{
    public class ProfileServiceIntegrationTests : IAsyncLifetime
    {
        private NpgsqlConnection _dbConnection = null!;
        private IDatabaseExecutor _dbExecutor = null!;
        private ProfileService _service = null!;
        private readonly string _connectionString;

        public ProfileServiceIntegrationTests()
        {
            _connectionString = "Host=localhost;Database=tellus_test;Username=postgres;Password=postgres;Port=5432";
        }

        public async Task InitializeAsync()
        {
            // Abrir conexão
            _dbConnection = new NpgsqlConnection(_connectionString);
            await _dbConnection.OpenAsync();

            // Registrar executor Dapper e serviço
            _dbExecutor = new DapperDatabaseExecutor(_dbConnection);
            _service = new ProfileService(_dbExecutor);

            // Registrar TypeHandler para List<Function>
            SqlMapper.AddTypeHandler(new JsonTypeHandler<List<Function>>());

            // Limpar e preparar banco
            await CleanDatabaseAsync();
            await SeedProfilesAsync();
        }

        public Task DisposeAsync()
        {
            _dbConnection.Close();
            return Task.CompletedTask;
        }

        private async Task CleanDatabaseAsync()
        {
            const string sql = @"
                TRUNCATE TABLE profiles RESTART IDENTITY;
                ALTER SEQUENCE profiles_id_seq RESTART WITH 1;";
            await _dbConnection.ExecuteAsync(sql);
        }

        private async Task SeedProfilesAsync()
        {
            const string sql = @"
                INSERT INTO profiles (name, active, functions) 
                VALUES 
                    ('Admin', true, '[]'::jsonb), 
                    ('Guest', false, '[]'::jsonb);";
            await _dbConnection.ExecuteAsync(sql);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnSeededProfiles()
        {
            var result = await _service.GetAllAsync();

            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Name == "Admin");
            result.Should().Contain(p => p.Name == "Guest");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSpecificProfile()
        {
            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result!.Name.Should().Be("Admin");
            result.Active.Should().BeTrue();
            result.Functions.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertAndReturnNewProfile()
        {
            var dto = new CreateProfileDto
            {
                Name = "Manager",
                Active = true,
                Functions = new List<Function>()
            };

            var created = await _service.CreateAsync(dto);

            created.Should().NotBeNull();
            created.Name.Should().Be("Manager");
            created.Active.Should().BeTrue();
            created.Functions.Should().BeEmpty();

            var all = await _service.GetAllAsync();
            all.Should().Contain(p => p.Name == "Manager");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingProfile()
        {
            var dto = new UpdateProfileDto
            {
                Name = "SuperAdmin",
                Active = true,
                Functions = new List<Function>()
            };

            var updated = await _service.UpdateAsync(1, dto);

            updated.Should().NotBeNull();
            updated!.Name.Should().Be("SuperAdmin");
            updated.Active.Should().BeTrue();
            updated.Functions.Should().BeEmpty();

            var reloaded = await _service.GetByIdAsync(1);
            reloaded!.Name.Should().Be("SuperAdmin");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProfile()
        {
            var deleted = await _service.DeleteAsync(2);

            deleted.Should().BeTrue();

            var result = await _service.GetAllAsync();
            result.Should().HaveCount(1);
            result.Should().NotContain(p => p.Id == 2);
        }
    }
}
