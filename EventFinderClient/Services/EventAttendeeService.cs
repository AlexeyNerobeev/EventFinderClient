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
                var result = await _apiService.PostAsync<object>($"eventattendees/events/{eventId}/register", registerDto);
                return true;
                }
                catch
                {
                    return false;
                }
            }

            public async Task<bool> CancelRegistrationAsync(int registrationId)
            {
                try
                {
                    return await _apiService.DeleteAsync($"eventattendees/{registrationId}");
                }
                catch
                {
                    return false;
                }
            }

            public async Task<int?> GetRegistrationIdAsync(int eventId, string email)
            {
                try
                {
                    var attendees = await GetAttendeesForEventAsync(eventId);
                    var registration = attendees?.FirstOrDefault(a => a.Email == email);
                    return registration?.Id;
                }
                catch
                {
                    return null;
                }
            }

        public async Task<bool> IsUserRegisteredForEventAsync(int eventId, string email)
        {
            try
            {
                var attendees = await GetAttendeesForEventAsync(eventId);

                var isRegistered = attendees?.Any(a =>
                    string.Equals(a.Email, email, StringComparison.OrdinalIgnoreCase)) ?? false;

                return isRegistered;
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
