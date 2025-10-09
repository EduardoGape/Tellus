using System.Net.Http.Json;
using TellusWeb.Application.Interfaces;
using TellusWeb.Domain.Entities;
using TellusWeb.Domain.DTOs;

namespace TellusWeb.Application.Services
{
    public class FunctionService : IFunctionService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/function"; 

        public FunctionService(HttpClient httpClient)
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

        public async Task<List<Function>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(BaseUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Function>>() ?? new List<Function>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting functions: {ex.Message}");
                return new List<Function>();
            }
        }

        public async Task<Function?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                    
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Function>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting function {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<Function> CreateAsync(CreateFunctionDto createDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(BaseUrl, createDto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Function>() ?? throw new Exception("Failed to create function");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating function: {ex.Message}");
                throw;
            }
        }

        public async Task<Function?> UpdateAsync(int id, UpdateFunctionDto updateDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", updateDto);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                    
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Function>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating function {id}: {ex.Message}");
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
                Console.WriteLine($"Error deleting function {id}: {ex.Message}");
                return false;
            }
        }
    }
}