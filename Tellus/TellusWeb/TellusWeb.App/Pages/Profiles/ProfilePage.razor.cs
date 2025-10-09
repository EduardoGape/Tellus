using Microsoft.AspNetCore.Components;
using TellusWeb.Application.Interfaces;
using TellusWeb.Domain.Entities;
using TellusWeb.Domain.DTOs;

namespace TellusWeb.App.Pages.Profiles
{
    public partial class ProfilePage : ComponentBase
    {
        [Inject]
        private IProfileService ProfileService { get; set; } = default!;

        [Inject]
        private IFunctionService FunctionService { get; set; } = default!;

        [Inject]
        private ITokenService TokenService { get; set; } = default!;

        [Inject]
        private ITokenDecoderService TokenDecoder { get; set; } = default!;

        [Inject]
        private IAuthorizationService AuthService { get; set; } = default!;

        private List<ProfileEntity> profiles = new();
        private List<Function> allFunctions = new();
        private bool showModal = false;
        private bool isEditing = false;
        private CreateProfileDto currentProfile = new();
        private int editingId = 0;
        private string errorMessage = string.Empty;
        
        private CurrentUser? currentUser;
        private bool canViewProfiles = false;
        private bool canEditProfiles = false;

        private bool CanSave => !string.IsNullOrWhiteSpace(currentProfile.Name);

        protected override async Task OnInitializedAsync()
        {
            currentUser = TokenDecoder.GetCurrentUser();
            
            if (currentUser != null)
            {
                canViewProfiles = await AuthService.CanAccessModuleAsync("Perfil");
                canEditProfiles = await AuthService.CanEditModuleAsync("Perfil");
                
                if (canViewProfiles)
                {
                    var token = TokenService.GetToken();
                    ProfileService.SetBearerToken(token);
                    FunctionService.SetBearerToken(token);
                    
                    await LoadProfiles();
                    await LoadAllFunctions();
                }
            }
        }

        private async Task LoadProfiles()
        {
            try
            {
                profiles = await ProfileService.GetAllAsync();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error loading profiles: {ex.Message}";
            }
            StateHasChanged();
        }

        private async Task LoadAllFunctions()
        {
            try
            {
                allFunctions = await FunctionService.GetAllAsync();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error loading functions: {ex.Message}";
            }
        }

        private void ShowCreateModal()
        {
            if (!canEditProfiles) return;
            
            isEditing = false;
            currentProfile = new CreateProfileDto { 
                Active = true,
                Functions = new List<Function>()
            };
            showModal = true;
            StateHasChanged();
        }

        private void ShowEditModal(ProfileEntity profile)
        {
            if (!canEditProfiles) return;
            
            isEditing = true;
            editingId = profile.Id;
            currentProfile = new CreateProfileDto
            {
                Name = profile.Name,
                Active = profile.Active,
                Functions = profile.Functions ?? new List<Function>()
            };
            showModal = true;
            StateHasChanged();
        }

        private void CloseModal()
        {
            showModal = false;
            ClearError();
            StateHasChanged();
        }

        private async Task SaveProfile()
        {
            if (!CanSave || !canEditProfiles) return;

            try
            {
                var token = TokenService.GetToken();
                ProfileService.SetBearerToken(token);
                
                if (isEditing)
                {
                    var updateDto = new UpdateProfileDto
                    {
                        Name = currentProfile.Name,
                        Active = currentProfile.Active,
                        Functions = currentProfile.Functions
                    };
                    await ProfileService.UpdateAsync(editingId, updateDto);
                }
                else
                {
                    await ProfileService.CreateAsync(currentProfile);
                }

                CloseModal();
                await LoadProfiles();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error saving profile: {ex.Message}";
            }
        }

        private async Task DeleteProfile(int id)
        {
            if (!canEditProfiles) return;
            
            try
            {
                var token = TokenService.GetToken();
                ProfileService.SetBearerToken(token);
                
                if (await ProfileService.DeleteAsync(id))
                {
                    await LoadProfiles();
                }
                else
                {
                    errorMessage = "Error deleting profile";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error deleting profile: {ex.Message}";
            }
        }

        private void ClearError()
        {
            errorMessage = string.Empty;
            StateHasChanged();
        }

        private void HandleCurrentProfileChanged(CreateProfileDto updatedProfile)
        {
            currentProfile = updatedProfile;
            StateHasChanged();
        }
    }
}