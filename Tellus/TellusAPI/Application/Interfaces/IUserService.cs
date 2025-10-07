using System.Collections.Generic;
using System.Threading.Tasks;
using TellusAPI.Domain.Entities;
using TellusAPI.Application.DTOs;
using TellusAPI.Application.Common;
using TellusAPI.Application.Filters;

namespace TellusAPI.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(CreateUserDto dto);
        Task<User?> UpdateAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<User>> SearchAsync(UserFilter filter);
        Task<string?> LoginAsync(string email, string password);
    }
}
