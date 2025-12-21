using EventFinderClient.Models.DTO;
using EventFinderClient.Services;
using EventFinderClient.Views;
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
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand RefreshEventsCommand { get; }
        public ICommand LoadEventsCommand { get; }
        public ICommand ViewDetailsCommand { get; }

        public EventsViewModel(IApiService apiService)
        {
            _apiService = apiService;
            _eventService = new EventService(apiService);

            RefreshEventsCommand = new Command(async () => await RefreshEventsAsync());
            LoadEventsCommand = new Command(async () => await LoadEventsAsync());
            ViewDetailsCommand = new Command<int>(async (eventId) => await ViewEventDetailsAsync(eventId));

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
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Ошибка",
                        "Не удалось загрузить мероприятия", "OK");
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ViewEventDetailsAsync(int eventId)
        {
            try
            {
                await Shell.Current.GoToAsync($"//EventDetailsPage?id={eventId}");
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Ошибка навигации",
                        $"Не удалось открыть детали мероприятия.\nОшибка: {ex.Message}",
                        "OK");
                });
            }
        }
    }
}
