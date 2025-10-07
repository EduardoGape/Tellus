using System.Collections.Generic;
using System.Threading.Tasks;
using TellusAPI.Application.DTOs;
using TellusAPI.Application.Interfaces;
using TellusAPI.Domain.Entities;

namespace TellusAPI.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IDatabaseExecutor _db;

        public ProfileService(IDatabaseExecutor db)
        {
            _db = db;
        }

        public Task<IEnumerable<Profile>> GetAllAsync()
        {
            const string sql = "SELECT * FROM profiles ORDER BY id";
            return _db.QueryAsync<Profile>(sql);
        }

        public Task<Profile?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM profiles WHERE id = @Id";
            return _db.QueryFirstOrDefaultAsync<Profile>(sql, new { Id = id });
        }

        public Task<Profile> CreateAsync(CreateProfileDto dto)
        {
            const string sql = @"
                INSERT INTO profiles (name, active, functions)
                VALUES (@Name, @Active, @Functions::jsonb)
                RETURNING *";
            
            return _db.QueryFirstAsync<Profile>(sql, dto);
        }

        public Task<Profile?> UpdateAsync(int id, UpdateProfileDto dto)
        {
            const string sql = @"
                UPDATE profiles
                SET name = @Name, active = @Active, functions = @Functions::jsonb
                WHERE id = @Id
                RETURNING *";

            return _db.QueryFirstOrDefaultAsync<Profile>(sql, new
            {
                Id = id,
                dto.Name,
                dto.Active,
                dto.Functions
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM profiles WHERE id = @Id";
            var affectedRows = await _db.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }
    }
}
