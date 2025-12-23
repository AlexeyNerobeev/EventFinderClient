using EventFinderClient.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFinderClient.Services
{
    public class AuthService
    {
        private readonly IApiService _apiService;

        public AuthService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                return await _apiService.PostAsync<AuthResponseDto>("auth/login", loginRequest);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка входа: {ex.Message}");
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                return await _apiService.PostAsync<AuthResponseDto>("auth/register", registerRequest);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка регистрации: {ex.Message}");
            }
        }

        public async Task<UserProfileDto> GetProfileAsync()
        {
            try
            {
                return await _apiService.GetAsync<UserProfileDto>("users/profile");
            }
            catch
            {
                return null;
            }
        }
    }
}
