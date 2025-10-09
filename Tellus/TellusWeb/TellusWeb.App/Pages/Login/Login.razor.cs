using Microsoft.AspNetCore.Components;
using TellusWeb.Application.Interfaces;
using TellusWeb.Domain.DTOs;

namespace TellusWeb.App.Pages.Login
{
    public partial class Login
    {
        [Inject]
        private IUserService UserService { get; set; } = default!;
        [Inject]
        private IFunctionService FunctionService { get; set; } = default!;
        [Inject]
        private IProfileService ProfileService { get; set; } = default!;

        [Inject]
        private ITokenService TokenService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private LoginDto loginModel = new();
        private string errorMessage = string.Empty;
        private bool isLoading = false;

        private async Task HandleLogin()
        {
            isLoading = true;
            errorMessage = string.Empty;
            StateHasChanged();

            try
            {
                var token = await UserService.LoginAsync(loginModel.Email, loginModel.Password);
                
                if (!string.IsNullOrEmpty(token))
                {
                    TokenService.SetToken(token);
                    
                    UserService.SetBearerToken(token);
                    FunctionService.SetBearerToken(token);
                    ProfileService.SetBearerToken(token);
                    
                    Navigation.NavigateTo("/home");
                }
                else
                {
                    errorMessage = "Invalid email or password";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
    }
}