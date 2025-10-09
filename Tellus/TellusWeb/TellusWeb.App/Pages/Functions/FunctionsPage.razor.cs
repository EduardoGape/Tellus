using Microsoft.AspNetCore.Components;
using TellusWeb.Application.Interfaces;
using TellusWeb.Domain.Entities;
using TellusWeb.Domain.DTOs;

namespace TellusWeb.App.Pages.Functions
{
    public partial class FunctionsPage
    {
        [Inject]
        private IFunctionService FunctionService { get; set; } = default!;

        [Inject]
        private ITokenService TokenService { get; set; } = default!;

        [Inject]
        private ITokenDecoderService TokenDecoder { get; set; } = default!;

        [Inject]
        private IAuthorizationService AuthService { get; set; } = default!;

        private List<Function> functions = new();
        private bool showModal = false;
        private bool isEditing = false;
        private CreateFunctionDto currentFunction = new();
        private int editingId = 0;
        private string errorMessage = string.Empty;
        
        private CurrentUser? currentUser;
        private bool canViewFunctions = false;
        private bool canEditFunctions = false;

        private bool CanSave => !string.IsNullOrWhiteSpace(currentFunction.Name);

        protected override async Task OnInitializedAsync()
        {
            currentUser = TokenDecoder.GetCurrentUser();
            
            if (currentUser != null)
            {
                canViewFunctions = await AuthService.CanAccessModuleAsync("Funções");
                canEditFunctions = await AuthService.CanEditModuleAsync("Funções");
                
                if (canViewFunctions)
                {
                    var token = TokenService.GetToken();
                    FunctionService.SetBearerToken(token);
                    await LoadFunctions();
                }
            }
        }

        private async Task LoadFunctions()
        {
            try
            {
                functions = await FunctionService.GetAllAsync();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error loading functions: {ex.Message}";
            }
            StateHasChanged();
        }

        private void ShowCreateModal()
        {
            if (!canEditFunctions) return;
            
            isEditing = false;
            currentFunction = new CreateFunctionDto { IsActive = true };
            showModal = true;
            StateHasChanged();
        }

        private void ShowEditModal(Function function)
        {
            if (!canEditFunctions) return;
            
            isEditing = true;
            editingId = function.Id;
            currentFunction = new CreateFunctionDto
            {
                Name = function.Name,
                CanRead = function.CanRead,
                CanWrite = function.CanWrite,
                IsActive = function.IsActive
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

        private async Task SaveFunction()
        {
            if (!CanSave || !canEditFunctions) return;

            try
            {
                var token = TokenService.GetToken();
                FunctionService.SetBearerToken(token);
                
                if (isEditing)
                {
                    var updateDto = new UpdateFunctionDto
                    {
                        Name = currentFunction.Name,
                        CanRead = currentFunction.CanRead,
                        CanWrite = currentFunction.CanWrite,
                        IsActive = currentFunction.IsActive
                    };
                    await FunctionService.UpdateAsync(editingId, updateDto);
                }
                else
                {
                    await FunctionService.CreateAsync(currentFunction);
                }

                CloseModal();
                await LoadFunctions();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error saving function: {ex.Message}";
            }
        }

        private async Task DeleteFunction(int id)
        {
            if (!canEditFunctions) return;
            
            try
            {
                var token = TokenService.GetToken();
                FunctionService.SetBearerToken(token);
                
                if (await FunctionService.DeleteAsync(id))
                {
                    await LoadFunctions();
                }
                else
                {
                    errorMessage = "Error deleting function";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error deleting function: {ex.Message}";
            }
        }

        private void ClearError()
        {
            errorMessage = string.Empty;
            StateHasChanged();
        }

        private void HandleCurrentFunctionChanged(CreateFunctionDto updatedFunction)
        {
            currentFunction = updatedFunction;
            StateHasChanged();
        }
    }
}