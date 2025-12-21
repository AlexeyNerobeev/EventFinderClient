using EventFinderClient.Models.DTO;
using EventFinderClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EventFinderClient.ViewModels
{
    public class EventDetailsViewModel : BaseViewModel
    {
        private readonly EventService _eventService;
        private readonly VenueService _venueService;
        private readonly OrganizerService _organizerService;

        private EventDetailsDto _event;
        private VenueDto _venue;
        private OrganizerDto _organizer;
        private int _eventId;

        public EventDetailsDto Event
        {
            get => _event;
            set
            {
                if (SetProperty(ref _event, value))
                {
                    if (value != null)
                    {
                        Task.Run(async () =>
                        {
                            await LoadRelatedDataAsync(value.VenueId, value.OrganizerId);
                        });
                    }
                }
            }
        }

        public VenueDto Venue
        {
            get => _venue;
            set => SetProperty(ref _venue, value);
        }

        public OrganizerDto Organizer
        {
            get => _organizer;
            set => SetProperty(ref _organizer, value);
        }

        public ICommand GoBackCommand { get; }

        public EventDetailsViewModel(IApiService apiService)
        {
            _eventService = new EventService(apiService);
            _venueService = new VenueService(apiService);
            _organizerService = new OrganizerService(apiService);

            GoBackCommand = new Command(async () => await GoBackAsync());
        }

        public int EventId
        {
            get => _eventId;
            set
            {
                if (_eventId != value && value > 0)
                {
                    _eventId = value;
                    OnPropertyChanged();
                    Task.Run(async () => await LoadEventDetailsAsync(value));
                }
            }
        }

        private async Task LoadEventDetailsAsync(int eventId)
        {
            try
            {
                IsBusy = true;

                var eventDetails = await _eventService.GetEventDetailsAsync(eventId);

                if (eventDetails != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Event = eventDetails;
                        Title = eventDetails.Title;
                    });
                }
            }
            catch
            {
                
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadRelatedDataAsync(int venueId, int organizerId)
        {
            try
            {
                var tasks = new System.Collections.Generic.List<Task>();

                if (venueId > 0)
                {
                    tasks.Add(LoadVenueAsync(venueId));
                }

                if (organizerId > 0)
                {
                    tasks.Add(LoadOrganizerAsync(organizerId));
                }

                await Task.WhenAll(tasks);
            }
            catch
            {
               
            }
        }

        private async Task LoadVenueAsync(int venueId)
        {
            try
            {
                var venue = await _venueService.GetVenueByIdAsync(venueId);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Venue = venue;
                });
            }
            catch
            {
               
            }
        }

        private async Task LoadOrganizerAsync(int organizerId)
        {
            try
            {
                var organizer = await _organizerService.GetOrganizerByIdAsync(organizerId);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Organizer = organizer;
                });
            }
            catch
            {
                
            }
        }

        private async Task GoBackAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("//EventsPage");
            }
            catch
            {
                
            }
        }
    }
}
