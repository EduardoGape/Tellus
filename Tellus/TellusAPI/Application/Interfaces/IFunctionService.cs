using TellusAPI.Domain.Entities;
using TellusAPI.Application.DTOs;

namespace TellusAPI.Application.Interfaces
{
    public interface IFunctionService
    {
        Task<IEnumerable<Function>> GetAllAsync();
        Task<Function?> GetByIdAsync(int id);
        Task<Function> CreateAsync(CreateFunctionDto createDto);
        Task<Function?> UpdateAsync(int id, UpdateFunctionDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}
