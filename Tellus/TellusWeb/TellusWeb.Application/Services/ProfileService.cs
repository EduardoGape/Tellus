using System.Net.Http.Json;
using TellusWeb.Application.Interfaces;
using TellusWeb.Domain.Entities;
using TellusWeb.Domain.DTOs;

namespace TellusWeb.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/profile";

        public ProfileService(HttpClient httpClient)
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

        public async Task<List<ProfileEntity>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ProfileEntity>>() ?? new List<ProfileEntity>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting profiles: {ex.Message}");
                return new List<ProfileEntity>();
            }
        }

        public async Task<ProfileEntity?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                    
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ProfileEntity>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting profile {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<ProfileEntity> CreateAsync(CreateProfileDto createDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, createDto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ProfileEntity>() ?? throw new Exception("Failed to create profile");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating profile: {ex.Message}");
                throw;
            }
        }

        public async Task<ProfileEntity?> UpdateAsync(int id, UpdateProfileDto updateDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", updateDto);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                    
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ProfileEntity>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile {id}: {ex.Message}");
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
                Console.WriteLine($"Error deleting profile {id}: {ex.Message}");
                return false;
            }
        }
    }
}