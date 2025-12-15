using EventFinderClient.Models.DTO;
using EventFinderClient.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EventFinderClient.ViewModels
{
    public class EventsViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly EventService _eventService;

        private ObservableCollection<EventDto> _events = new();
        private EventDto _selectedEvent;
        private bool _isRefreshing;

        public ObservableCollection<EventDto> Events
        {
            get => _events;
            set => SetProperty(ref _events, value);
        }

        public EventDto SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                SetProperty(ref _selectedEvent, value);
                if (value != null)
                {
                    Task.Run(async () => await OnEventSelectedAsync());
                }
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand RefreshEventsCommand { get; }
        public ICommand LoadEventsCommand { get; }

        public EventsViewModel(IApiService apiService)
        {
            _apiService = apiService;
            _eventService = new EventService(apiService);

            RefreshEventsCommand = new Command(async () => await RefreshEventsAsync());
            LoadEventsCommand = new Command(async () => await LoadEventsAsync());

            Task.Run(async () => await LoadEventsAsync());
        }

        private async Task RefreshEventsAsync()
        {
            IsRefreshing = true;
            await LoadEventsAsync();
            IsRefreshing = false;
        }

        private async Task LoadEventsAsync()
        {
            try
            {
                IsBusy = true;

                var eventsList = await _eventService.GetEventsAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Events.Clear();
                    foreach (var eventItem in eventsList)
                    {
                        Events.Add(eventItem);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки событий: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnEventSelectedAsync()
        {
            if (SelectedEvent == null)
                return;

            await Shell.Current.GoToAsync($"EventDetailsPage?id={SelectedEvent.Id}");
            SelectedEvent = null;
        }
    }
}
