using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFinderClient.Models.DTO
{
    public class CreateOrganizerDto
    {
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
