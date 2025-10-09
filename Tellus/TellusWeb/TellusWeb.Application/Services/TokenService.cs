using TellusWeb.Application.Interfaces;
using Microsoft.JSInterop;

namespace TellusWeb.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IJSRuntime _jsRuntime;
        private string _token = string.Empty;
        private const string TokenKey = "authToken";

        public TokenService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            _token = await GetTokenFromStorage();
        }

        public string GetToken() => _token;

        public async void SetToken(string token)
        {
            _token = token;
            await SaveTokenToStorage(token);
        }

        public async void ClearToken()
        {
            _token = string.Empty;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey); 
        }

        private async Task<string> GetTokenFromStorage()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey) ?? string.Empty; 
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task SaveTokenToStorage(string token)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token); 
            }
            catch
            {
                
            }
        }
    }
}