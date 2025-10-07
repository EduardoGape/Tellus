using System.Collections.Generic;
using System.Threading.Tasks;
using TellusAPI.Application.DTOs;
using TellusAPI.Domain.Entities;

namespace TellusAPI.Application.Interfaces
{
    public interface IProfileService
    {
        Task<IEnumerable<Profile>> GetAllAsync();
        Task<Profile?> GetByIdAsync(int id);
        Task<Profile> CreateAsync(CreateProfileDto dto);
        Task<Profile?> UpdateAsync(int id, UpdateProfileDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
