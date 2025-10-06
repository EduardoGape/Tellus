using Xunit;
using FluentAssertions;
using TellusAPI.Application.Services;
using TellusAPI.Domain.Entities;
using System.Data;
using Npgsql;
using Dapper;

namespace TellusAPI.IntegrationTests.Services
{
    public class HelloWorldServiceIntegrationTests : IAsyncLifetime
    {
        private IDbConnection _dbConnection = null!;
        private HelloWorldService _helloWorldService = null!;
        private readonly string _connectionString;

        public HelloWorldServiceIntegrationTests()
        {
            _connectionString = "Host=localhost;Database=tellus_test;Username=postgres;Password=postgres;Port=5432";
        }

        public async Task InitializeAsync()
        {
            var npgsqlConnection = new NpgsqlConnection(_connectionString);
            await npgsqlConnection.OpenAsync();
            _dbConnection = npgsqlConnection;
            
            _helloWorldService = new HelloWorldService(_dbConnection);

            await CleanDatabaseAsync();
            await SeedTestDataAsync();
        }

        public async Task DisposeAsync()
        {
            await CleanDatabaseAsync();
            _dbConnection?.Dispose();
        }

        private async Task CleanDatabaseAsync()
        {
            var sql = @"
                TRUNCATE TABLE helloworld RESTART IDENTITY;
                ALTER SEQUENCE helloworld_id_seq RESTART WITH 1;";
            await _dbConnection.ExecuteAsync(sql);
        }

        private async Task SeedTestDataAsync()
        {
            var sql = @"
                INSERT INTO helloworld (name) VALUES 
                ('Test Item 1'),
                ('Test Item 2'),
                ('Test Item 3')";

            await _dbConnection.ExecuteAsync(sql);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnThreeItems()
        {
            // Act
            var result = await _helloWorldService.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(x => x.Name == "Test Item 1");
            result.Should().Contain(x => x.Name == "Test Item 2");
            result.Should().Contain(x => x.Name == "Test Item 3");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCorrectIds()
        {
            // Act
            var result = await _helloWorldService.GetAllAsync();

            // Assert
            // Verifica se os IDs são 1, 2, 3 em qualquer ordem
            result.Select(x => x.Id).Should().BeEquivalentTo(new[] { 1, 2, 3 });
            
            // OU verifica na ordem específica:
            // result.Select(x => x.Id).Should().Equal(new[] { 1, 2, 3 });
        }
    }
}