using System.Collections.Generic;
using System.Threading.Tasks;
using TellusWeb.Domain.Entities;
using TellusWeb.Domain.DTOs;

namespace TellusWeb.Application.Interfaces
{
    public interface IProfileService
    {
        void SetBearerToken(string token);
        Task<List<ProfileEntity>> GetAllAsync();
        Task<ProfileEntity?> GetByIdAsync(int id);
        Task<ProfileEntity> CreateAsync(CreateProfileDto createDto);
        Task<ProfileEntity?> UpdateAsync(int id, UpdateProfileDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}