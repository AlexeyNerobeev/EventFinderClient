using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFinderClient.Models
{
    public class EventAttendee
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;
    }
}
