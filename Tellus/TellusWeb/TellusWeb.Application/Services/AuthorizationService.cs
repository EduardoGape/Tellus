using TellusWeb.Application.Interfaces;
using TellusWeb.Domain.Entities;

namespace TellusWeb.Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ITokenDecoderService _tokenDecoder;
        private readonly IProfileService _profileService;
        private CurrentUser? _currentUser;
        private ProfileEntity? _currentProfile;

        public AuthorizationService(ITokenDecoderService tokenDecoder, IProfileService profileService)
        {
            _tokenDecoder = tokenDecoder;
            _profileService = profileService;
        }

        private async Task LoadCurrentProfileAsync()
        {
            _currentUser = _tokenDecoder.GetCurrentUser();
            if (_currentUser != null)
            {
                var profiles = await _profileService.GetAllAsync();
                _currentProfile = profiles.FirstOrDefault(p => p.Id == _currentUser.ProfileId);
            }
        }

        public async Task<bool> CanAccessModuleAsync(string moduleName)
        {
            await LoadCurrentProfileAsync();
            if (_currentProfile == null) return false;
            
            return _currentProfile.Functions.Any(f => 
                f.Name.Contains(moduleName) && f.CanRead && f.IsActive);
        }

        public async Task<bool> CanEditModuleAsync(string moduleName)
        {
            await LoadCurrentProfileAsync();
            if (_currentProfile == null) return false;
            
            return _currentProfile.Functions.Any(f => 
                f.Name.Contains(moduleName) && f.CanWrite && f.IsActive);
        }

        public async Task<List<Function>> GetUserFunctionsAsync()
        {
            await LoadCurrentProfileAsync();
            return _currentProfile?.Functions?.Where(f => f.IsActive).ToList() ?? new List<Function>();
        }
    }
}