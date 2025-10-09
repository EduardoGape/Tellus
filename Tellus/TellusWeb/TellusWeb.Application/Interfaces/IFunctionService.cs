using TellusWeb.Domain.Entities;
using TellusWeb.Domain.DTOs;

namespace TellusWeb.Application.Interfaces
{
    public interface IFunctionService
    {
        
        void SetBearerToken(string token);
        Task<List<Function>> GetAllAsync();
        Task<Function?> GetByIdAsync(int id);
        Task<Function> CreateAsync(CreateFunctionDto createDto);
        Task<Function?> UpdateAsync(int id, UpdateFunctionDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}