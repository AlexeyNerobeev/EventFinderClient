using EventFinderClient.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFinderClient.Services
{
    public class VenueService
    {
        private readonly IApiService _apiService;

        public VenueService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<VenueDto> GetVenueByIdAsync(int venueId)
        {
            try
            {
                if (venueId <= 0)
                    return null;

                return await _apiService.GetAsync<VenueDto>($"venues/{venueId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения места проведения: {ex.Message}");
                return null;
            }
        }

        public async Task<List<VenueDto>> GetAllVenuesAsync()
        {
            try
            {
                return await _apiService.GetAsync<List<VenueDto>>("venues");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения мест проведения: {ex.Message}");
                return new List<VenueDto>();
            }
        }
    }
}
