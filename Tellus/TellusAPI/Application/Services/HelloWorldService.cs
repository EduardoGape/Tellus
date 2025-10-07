using Dapper;
using TellusAPI.Application.Interfaces;
using TellusAPI.Domain.Entities;
using TellusAPI.Application.DTOs;
using System.Data;

namespace TellusAPI.Application.Services
{
    public class HelloWorldService : IHelloWorldService
    {
        private readonly IDbConnection _dbConnection;

        public HelloWorldService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<string> GetHelloWorldMessage()
        {
            return await Task.FromResult("Hello World from Tellus API!");

        }

        public async Task<IEnumerable<HelloWorld>> GetAllAsync()
        {
            var sql = "SELECT id, name, created_at as CreatedAt FROM helloworld ORDER BY created_at DESC";
            return await _dbConnection.QueryAsync<HelloWorld>(sql);
        }

        public async Task<HelloWorld?> GetByIdAsync(int id)
        {
            var sql = "SELECT id, name, created_at as CreatedAt FROM helloworld WHERE id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<HelloWorld>(sql, new { Id = id });
        }

        public async Task<HelloWorld> CreateAsync(CreateHelloWorldDto createDto)
        {
            var sql = @"
                INSERT INTO helloworld (name) 
                VALUES (@Name) 
                RETURNING id, name, created_at as CreatedAt";

            return await _dbConnection.QueryFirstAsync<HelloWorld>(sql, new { Name = createDto.Name });
        }

        public async Task<HelloWorld?> UpdateAsync(int id, UpdateHelloWorldDto updateDto)
        {
            var sql = @"
                UPDATE helloworld 
                SET name = @Name 
                WHERE id = @Id 
                RETURNING id, name, created_at as CreatedAt";

            return await _dbConnection.QueryFirstOrDefaultAsync<HelloWorld>(sql, new 
            { 
                Id = id, 
                Name = updateDto.Name 
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM helloworld WHERE id = @Id";
            var affectedRows = await _dbConnection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }
    }
}