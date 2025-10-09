using System.Net.Http.Json;
using TellusWeb.Application.Interfaces;
using TellusWeb.Domain.Entities;
using TellusWeb.Domain.DTOs;
using TellusWeb.Domain.Common;
using TellusWeb.Domain.Filters;

namespace TellusWeb.Application.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/user";

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public void SetBearerToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<User>>() ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting users: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                    
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<User> CreateAsync(CreateUserDto createDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, createDto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>() ?? throw new Exception("Failed to create user");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                throw;
            }
        }

        public async Task<User?> UpdateAsync(int id, UpdateUserDto updateDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", updateDto);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                    
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;
                    
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<PagedResult<User>> SearchAsync(UserFilter filter)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/search?{ToQueryString(filter)}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PagedResult<User>>() ?? new PagedResult<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching users: {ex.Message}");
                return new PagedResult<User>();
            }
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            try
            {
                var loginDto = new LoginDto { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", loginDto);
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return null;
                    
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result?.Token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return null;
            }
        }

        private string ToQueryString(UserFilter filter)
        {
            var queryParams = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(filter.Name))
                queryParams.Add($"Name={Uri.EscapeDataString(filter.Name)}");
                
            if (!string.IsNullOrWhiteSpace(filter.Email))
                queryParams.Add($"Email={Uri.EscapeDataString(filter.Email)}");
                
            if (filter.ProfileId.HasValue)
                queryParams.Add($"ProfileId={filter.ProfileId.Value}");
                
            queryParams.Add($"Page={filter.Page}");
            queryParams.Add($"PageSize={filter.PageSize}");
            
            return string.Join("&", queryParams);
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}