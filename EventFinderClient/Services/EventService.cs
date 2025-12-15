using EventFinderClient.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFinderClient.Services
{
    public class EventService
    {
        private readonly IApiService _apiService;

        public EventService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<EventDto>> GetEventsAsync()
        {
            try
            {
                return await _apiService.GetAsync<List<EventDto>>("events");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения событий: {ex.Message}");
            }
        }

        public async Task<EventDetailsDto> GetEventDetailsAsync(int eventId)
        {
            try
            {
                return await _apiService.GetAsync<EventDetailsDto>($"events/{eventId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения деталей события: {ex.Message}");
            }
        }

        public async Task<bool> RegisterForEventAsync(int eventId, RegisterForEventDto registerDto)
        {
            try
            {
                var result = await _apiService.PostAsync<object>($"events/{eventId}/register", registerDto);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка регистрации на событие: {ex.Message}");
            }
        }

        public async Task<bool> CancelRegistrationAsync(int eventId)
        {
            try
            {
                return await _apiService.DeleteAsync($"events/{eventId}/register");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка отмены регистрации: {ex.Message}");
            }
        }
    }
}
