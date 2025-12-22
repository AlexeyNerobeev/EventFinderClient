using EventFinderClient.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFinderClient.Services
{
    public class EventAttendeeService
    {
        private readonly IApiService _apiService;

        public EventAttendeeService(IApiService apiService)
        {
            _apiService = apiService;
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
                return await _apiService.DeleteAsync($"events/{eventId}/register?email={Uri.EscapeDataString(email)}");
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
                var attendees = await GetAttendeesForEventAsync(eventId);
                return attendees?.Any(a => a.Email == email) ?? false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<EventAttendeeDto>> GetAttendeesForEventAsync(int eventId)
        {
            try
            {
                return await _apiService.GetAsync<List<EventAttendeeDto>>($"eventattendees/events/{eventId}");
            }
            catch
            {
                return new List<EventAttendeeDto>();
            }
        }
    }
}
