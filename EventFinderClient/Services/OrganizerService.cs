using EventFinderClient.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFinderClient.Services
{
    public class OrganizerService
    {
        private readonly IApiService _apiService;

        public OrganizerService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<OrganizerDto> GetOrganizerByIdAsync(int organizerId)
        {
            try
            {
                if (organizerId <= 0)
                    return null;

                return await _apiService.GetAsync<OrganizerDto>($"organizers/{organizerId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения организатора: {ex.Message}");
                return null;
            }
        }

        public async Task<List<OrganizerDto>> GetAllOrganizersAsync()
        {
            try
            {
                return await _apiService.GetAsync<List<OrganizerDto>>("organizers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения организаторов: {ex.Message}");
                return new List<OrganizerDto>();
            }
        }
    }
}
