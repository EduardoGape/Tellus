using System.Collections.Generic;
using System.Threading.Tasks;
using TellusAPI.Application.DTOs;
using TellusAPI.Domain.Entities;
using TellusAPI.Application.Interfaces;

namespace TellusAPI.Application.Services
{
    public class FunctionService : IFunctionService
    {
        private readonly IDatabaseExecutor _db;

        public FunctionService(IDatabaseExecutor db)
        {
            _db = db;
        }

        public Task<IEnumerable<Function>> GetAllAsync()
        {
            var sql = @"
                SELECT id, 
                       name, 
                       can_read AS ""CanRead"", 
                       can_write AS ""CanWrite"", 
                       is_active AS ""IsActive""
                FROM functions
                ORDER BY id";
            return _db.QueryAsync<Function>(sql);
        }

        public Task<Function?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT id, 
                       name, 
                       can_read AS ""CanRead"", 
                       can_write AS ""CanWrite"", 
                       is_active AS ""IsActive""
                FROM functions
                WHERE id = @Id";
            return _db.QueryFirstOrDefaultAsync<Function>(sql, new { Id = id });
        }

        public Task<Function> CreateAsync(CreateFunctionDto dto)
        {
            var sql = @"
                INSERT INTO functions (name, can_read, can_write, is_active)
                VALUES (@Name, @CanRead, @CanWrite, @IsActive)
                RETURNING id, name, can_read AS ""CanRead"", can_write AS ""CanWrite"", is_active AS ""IsActive""";
            return _db.QueryFirstAsync<Function>(sql, dto);
        }

        public Task<Function?> UpdateAsync(int id, UpdateFunctionDto dto)
        {
            var sql = @"
                UPDATE functions
                SET name = @Name, can_read = @CanRead, can_write = @CanWrite, is_active = @IsActive
                WHERE id = @Id
                RETURNING id, name, can_read AS ""CanRead"", can_write AS ""CanWrite"", is_active AS ""IsActive""";
            return _db.QueryFirstOrDefaultAsync<Function>(sql, new { Id = id, dto.Name, dto.CanRead, dto.CanWrite, dto.IsActive });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM functions WHERE id = @Id";
            var affectedRows = await _db.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }
    }
}
