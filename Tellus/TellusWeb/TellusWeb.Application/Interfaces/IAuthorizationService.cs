using TellusWeb.Domain.Entities;

namespace TellusWeb.Application.Interfaces
{
    public interface IAuthorizationService
    {
        Task<bool> CanAccessModuleAsync(string moduleName);
        Task<bool> CanEditModuleAsync(string moduleName);
        Task<List<Function>> GetUserFunctionsAsync();
    }
}