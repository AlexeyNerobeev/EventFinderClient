using EventFinderClient.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
            catch
            {
                return new List<EventDto>();
            }
        }

        public async Task<EventDetailsDto> GetEventDetailsAsync(int eventId)
        {
            try
            {
                return await _apiService.GetAsync<EventDetailsDto>($"events/{eventId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> RegisterForEventAsync(int eventId, RegisterForEventDto registerDto)
        {
            try
            {
                var result = await _apiService.PostAsync<object>($"events/{eventId}/register", registerDto);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelRegistrationAsync(int eventId, string email)
        {
            try
            {
                return await _apiService.DeleteAsync($"events/{eventId}/register?email={email}");
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsUserRegisteredForEventAsync(int eventId, string email)
        {
            try
            {
                var attendees = await _apiService.GetAsync<List<EventAttendeeDto>>($"eventattendees/events/{eventId}");
                return attendees?.Any(a => a.Email == email) ?? false;
            }
            catch
            {
                return false;
            }
        }
    }
}
