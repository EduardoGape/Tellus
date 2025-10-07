using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using TellusAPI.Application.DTOs;
using TellusAPI.Application.Interfaces;
using TellusAPI.Domain.Entities;
using TellusAPI.Domain.Entities.Reference;
using System.Data;
using TellusAPI.Application.Common;
using TellusAPI.Application.Filters;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;


namespace TellusAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IDatabaseExecutor _db;
        private readonly IConfiguration _configuration;

        public UserService(IDatabaseExecutor db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            const string sql = "SELECT * FROM users ORDER BY id";
            return _db.QueryAsync<User>(sql);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            const string sql = "SELECT * FROM users WHERE id = @Id";
            return _db.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public Task<User> CreateAsync(CreateUserDto dto)
        {
            const string sql = @"
                INSERT INTO users (name, email, password, profile)
                VALUES (@Name, @Email, @Password, @Profile)
                RETURNING *";

            var parameters = new DynamicParameters();
            parameters.Add("Name", dto.Name);
            parameters.Add("Email", dto.Email);
            parameters.Add("Password", dto.Password);
            parameters.Add("Profile", dto.Profile); // ✅ direto, JsonTypeHandler cuida do JSONB

            return _db.QueryFirstAsync<User>(sql, parameters);
        }

        public Task<User?> UpdateAsync(int id, UpdateUserDto dto)
        {
            const string sql = @"
                UPDATE users
                SET name = @Name, email = @Email, password = @Password, profile = @Profile
                WHERE id = @Id
                RETURNING *";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("Name", dto.Name);
            parameters.Add("Email", dto.Email);
            parameters.Add("Password", dto.Password);
            parameters.Add("Profile", dto.Profile); // ✅ direto

            return _db.QueryFirstOrDefaultAsync<User>(sql, parameters);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM users WHERE id = @Id";
            var affectedRows = await _db.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<PagedResult<User>> SearchAsync(UserFilter filter)
        {
            var sql = "SELECT * FROM users WHERE 1=1";
            var countSql = "SELECT COUNT(*) FROM users WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                sql += " AND LOWER(name) LIKE LOWER(@Name)";
                countSql += " AND LOWER(name) LIKE LOWER(@Name)";
                parameters.Add("Name", $"%{filter.Name}%");
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                sql += " AND LOWER(email) LIKE LOWER(@Email)";
                countSql += " AND LOWER(email) LIKE LOWER(@Email)";
                parameters.Add("Email", $"%{filter.Email}%");
            }

            if (filter.ProfileId.HasValue)
            {
                sql += " AND (profile->>'Id')::int = @ProfileId";
                countSql += " AND (profile->>'Id')::int = @ProfileId";
                parameters.Add("ProfileId", filter.ProfileId.Value);
            }

            var total = await _db.QueryFirstAsync<int>(countSql, parameters);

            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1 ? 20 : filter.PageSize;
            var offset = (page - 1) * pageSize;

            sql += " ORDER BY id LIMIT @PageSize OFFSET @Offset";
            parameters.Add("PageSize", pageSize);
            parameters.Add("Offset", offset);

            var items = await _db.QueryAsync<User>(sql, parameters);

            return new PagedResult<User>
            {
                Items = items.AsList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }
        
        public async Task<string?> LoginAsync(string email, string password)
        {
            const string sql = "SELECT * FROM users WHERE email = @Email AND password = @Password";
            var user = await _db.QueryFirstOrDefaultAsync<User>(sql, new { Email = email, Password = password });

            if (user == null)
                return null;

            var key = Encoding.ASCII.GetBytes(_configuration["JWT:SecretKey"]!);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
