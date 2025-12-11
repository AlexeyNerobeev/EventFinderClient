using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFinderClient.Models
{
    public class Organizer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ObservableCollection<Event> Events { get; set; } = new ObservableCollection<Event>();
    }
}
