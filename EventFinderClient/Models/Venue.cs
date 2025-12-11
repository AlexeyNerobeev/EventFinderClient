using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFinderClient.Models
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public ObservableCollection<Event> Events { get; set; } = new ObservableCollection<Event>();
    }
}
