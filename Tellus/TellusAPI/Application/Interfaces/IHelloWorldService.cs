using TellusAPI.Domain.Entities;
using TellusAPI.Application.DTOs;

namespace TellusAPI.Application.Interfaces
{
    public interface IHelloWorldService
    {
        Task<string> GetHelloWorldMessage();
        Task<IEnumerable<HelloWorld>> GetAllAsync();
        Task<HelloWorld?> GetByIdAsync(int id);
        Task<HelloWorld> CreateAsync(CreateHelloWorldDto createDto);
        Task<HelloWorld?> UpdateAsync(int id, UpdateHelloWorldDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}

