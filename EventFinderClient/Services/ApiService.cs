using EventFinderClient.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventFinderClient.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseUrl = "https://localhost:7117/api";

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);

                if (result?.Success == true)
                {
                    return result.Data;
                }

                throw new Exception(result?.Error?.Message ?? "Unknown error");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Network error: {ex.Message}", ex);
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(errorContent, _jsonOptions);
                    throw new Exception(errorResult?.Error?.Message ?? $"HTTP Error: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, _jsonOptions);

                if (result?.Success == true)
                {
                    return result.Data;
                }

                throw new Exception(result?.Error?.Message ?? "Unknown error");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Network error: {ex.Message}", ex);
            }
        }

        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, _jsonOptions);

                if (result?.Success == true)
                {
                    return result.Data;
                }

                throw new Exception(result?.Error?.Message ?? "Unknown error");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Network error: {ex.Message}", ex);
            }
        }

        public async Task<T> DeleteAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);

                if (result?.Success == true)
                {
                    return result.Data;
                }

                throw new Exception(result?.Error?.Message ?? "Unknown error");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Network error: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
                    return result?.Success == true;
                }

                return false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public void SetAuthorizationHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
        }

        public async Task<AuthResponseDto> GetProfileAsync()
        {
            try
            {
                return await GetAsync<AuthResponseDto>("users/profile");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения профиля: {ex.Message}");
            }
        }
    }

    internal class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public ApiError? Error { get; set; }
    }

    internal class ApiError
    {
        public string? Message { get; set; }
        public string? Code { get; set; }
    }
}
