using System.Collections.Generic;
using System.Threading.Tasks;
using TellusWeb.Domain.Entities;
using TellusWeb.Domain.DTOs;
using TellusWeb.Domain.Common;
using TellusWeb.Domain.Filters;

namespace TellusWeb.Application.Interfaces
{
    public interface IUserService
    {
        void SetBearerToken(string token);
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(CreateUserDto createDto);
        Task<User?> UpdateAsync(int id, UpdateUserDto updateDto);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<User>> SearchAsync(UserFilter filter);
        Task<string?> LoginAsync(string email, string password);
    }
}