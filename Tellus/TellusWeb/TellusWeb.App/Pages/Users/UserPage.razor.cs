using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TellusWeb.Application.Interfaces;
using TellusWeb.Domain.Entities;
using TellusWeb.Domain.DTOs;
using TellusWeb.Domain.Entities.Reference;
using TellusWeb.Domain.Filters;
using TellusWeb.Domain.Common;
using System.Threading;

namespace TellusWeb.App.Pages.Users
{
    public partial class UserPage : ComponentBase, IDisposable
    {
        [Inject]
        private IUserService UserService { get; set; } = default!;

        [Inject]
        private IProfileService ProfileService { get; set; } = default!;

        [Inject]
        private ITokenService TokenService { get; set; } = default!;

        [Inject]
        private ITokenDecoderService TokenDecoder { get; set; } = default!;

        [Inject]
        private IAuthorizationService AuthService { get; set; } = default!;

        private List<User> users = new List<User>();
        private List<ProfileEntity> profilesList = new List<ProfileEntity>();
        private PagedResult<User> pagedResult = new PagedResult<User>();
        private UserFilter searchFilter = new UserFilter();
        private bool showModal = false;
        private bool isEditing = false;
        private CreateUserDto currentUserDto = new CreateUserDto();
        private int editingId = 0;
        private string errorMessage = string.Empty;
        private bool isSearching = false;
        private bool hasSearched = false;
        private CancellationTokenSource? searchCancellationTokenSource;
        
        private CurrentUser? currentUser;
        private bool canViewUsers = false;
        private bool canEditUsers = false;

        private bool CanSave => !string.IsNullOrWhiteSpace(currentUserDto.Name) && 
                               !string.IsNullOrWhiteSpace(currentUserDto.Email) && 
                               !string.IsNullOrWhiteSpace(currentUserDto.Password) &&
                               currentUserDto.Profile?.Id > 0;

        protected override async Task OnInitializedAsync()
        {
            currentUser = TokenDecoder.GetCurrentUser();
            
            if (currentUser != null)
            {
                canViewUsers = await AuthService.CanAccessModuleAsync("Usuários");
                canEditUsers = await AuthService.CanEditModuleAsync("Usuários");
                
                if (canViewUsers)
                {
                    var token = TokenService.GetToken();
                    UserService.SetBearerToken(token);
                    ProfileService.SetBearerToken(token);
                    
                    await LoadProfiles();
                    await SearchUsers();
                }
            }
        }

        private async Task SearchUsers()
        {
            if (!canViewUsers) return;

            isSearching = true;
            hasSearched = true;
            StateHasChanged();

            try
            {
                pagedResult = await UserService.SearchAsync(searchFilter);
                users = pagedResult.Items?.ToList() ?? new List<User>();

                
                if (users.Count == 0 && pagedResult.TotalPages > 0 && searchFilter.Page > 1)
                {
                    searchFilter.Page = pagedResult.TotalPages;
                    pagedResult = await UserService.SearchAsync(searchFilter);
                    users = pagedResult.Items?.ToList() ?? new List<User>();
                }
            }
            catch (Exception exception)
            {
                errorMessage = $"Erro ao buscar usuários: {exception.Message}";
                users = new List<User>();
                pagedResult = new PagedResult<User>();
            }
            finally
            {
                isSearching = false;
                StateHasChanged();
            }
        }

        private async Task LoadProfiles()
        {
            try
            {
                profilesList = await ProfileService.GetAllAsync();
            }
            catch (Exception exception)
            {
                errorMessage = $"Erro ao carregar perfis: {exception.Message}";
            }
        }

        private async void OnSearchInput(ChangeEventArgs eventArgs)
        {
            
            searchCancellationTokenSource?.Cancel();
            searchCancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                await Task.Delay(500, searchCancellationTokenSource.Token);
                searchFilter.Page = 1; 
                await SearchUsers();
            }
            catch (TaskCanceledException)
            {
                
            }
        }

        private async void OnSearchKeyPress(KeyboardEventArgs keyboardEventArgs)
        {
            if (keyboardEventArgs.Key == "Enter")
            {
                searchCancellationTokenSource?.Cancel();
                searchFilter.Page = 1;
                await SearchUsers();
            }
        }

        private async void HandleProfileChange(ChangeEventArgs eventArgs)
        {
            
            if (eventArgs.Value != null)
            {
                if (string.IsNullOrEmpty(eventArgs.Value.ToString()))
                {
                    searchFilter.ProfileId = null;
                }
                else if (int.TryParse(eventArgs.Value.ToString(), out int profileId))
                {
                    searchFilter.ProfileId = profileId;
                }
            }
            
            searchCancellationTokenSource?.Cancel();
            searchFilter.Page = 1;
            await SearchUsers();
        }

        private async Task ChangePage(int page)
        {
            if (page < 1 || page > pagedResult.TotalPages) return;
            
            searchFilter.Page = page;
            await SearchUsers();
        }

        private async Task ClearFilters()
        {
            searchFilter = new UserFilter();
            await SearchUsers();
        }

        private void ShowCreateModal()
        {
            if (!canEditUsers) return;
            
            isEditing = false;
            currentUserDto = new CreateUserDto 
            { 
                Profile = new ProfileReference()
            };
            showModal = true;
            StateHasChanged();
        }

        private void ShowEditModal(User user)
        {
            if (!canEditUsers) return;
            
            isEditing = true;
            editingId = user.Id;
            currentUserDto = new CreateUserDto
            {
                Name = user.Name,
                Email = user.Email,
                Password = "", // Não mostrar senha existente por segurança
                Profile = user.Profile
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

        private async Task SaveUser()
        {
            if (!CanSave || !canEditUsers) return;

            try
            {
                
                var token = TokenService.GetToken();
                UserService.SetBearerToken(token);
                
                if (isEditing)
                {
                    var updateDto = new UpdateUserDto
                    {
                        Name = currentUserDto.Name,
                        Email = currentUserDto.Email,
                        Password = currentUserDto.Password,
                        Profile = currentUserDto.Profile
                    };
                    await UserService.UpdateAsync(editingId, updateDto);
                }
                else
                {
                    await UserService.CreateAsync(currentUserDto);
                }

                CloseModal();
                await SearchUsers(); 
            }
            catch (Exception exception)
            {
                errorMessage = $"Erro ao salvar usuário: {exception.Message}";
            }
        }

        private async Task DeleteUser(int id)
        {
            if (!canEditUsers) return;
            
            try
            {
                
                var token = TokenService.GetToken();
                UserService.SetBearerToken(token);
                
                if (await UserService.DeleteAsync(id))
                {
                    await SearchUsers(); 
                }
                else
                {
                    errorMessage = "Erro ao deletar usuário";
                }
            }
            catch (Exception exception)
            {
                errorMessage = $"Erro ao deletar usuário: {exception.Message}";
            }
        }

        private void ClearError()
        {
            errorMessage = string.Empty;
            StateHasChanged();
        }

        private void HandleCurrentUserChanged(CreateUserDto updatedUser)
        {
            currentUserDto = updatedUser;
            StateHasChanged();
        }

        public void Dispose()
        {
            searchCancellationTokenSource?.Cancel();
            searchCancellationTokenSource?.Dispose();
        }
    }
}