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
        private readonly IApiService _apiService;
        private readonly EventService _eventService;

        private EventDetailsDto _event;
        private int _eventId;
        private bool _isRegistered;

        public EventDetailsDto Event
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        public int EventId
        {
            get => _eventId;
            set
            {
                if (_eventId != value)
                {
                    _eventId = value;
                    OnPropertyChanged();
                    if (value > 0)
                    {
                        Task.Run(async () => await LoadEventDetailsAsync());
                    }
                }
            }
        }

        public bool IsRegistered
        {
            get => _isRegistered;
            set => SetProperty(ref _isRegistered, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand CancelRegistrationCommand { get; }

        public EventDetailsViewModel(IApiService apiService)
        {
            _apiService = apiService;
            _eventService = new EventService(apiService);

            RegisterCommand = new Command(async () => await RegisterForEventAsync());
            CancelRegistrationCommand = new Command(async () => await CancelRegistrationAsync());
        }

        private async Task LoadEventDetailsAsync()
        {
            try
            {
                IsBusy = true;

                var eventDetails = await _eventService.GetEventDetailsAsync(EventId);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Event = eventDetails;
                    IsRegistered = new Random().Next(0, 2) == 1;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки деталей события: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RegisterForEventAsync()
        {
            try
            {
                IsBusy = true;

                var registerDto = new RegisterForEventDto
                {
                    UserName = "Текущий пользователь",
                    Email = "user@example.com"
                };

                var success = await _eventService.RegisterForEventAsync(EventId, registerDto);

                if (success)
                {
                    IsRegistered = true;
                    await LoadEventDetailsAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка регистрации на событие: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CancelRegistrationAsync()
        {
            try
            {
                IsBusy = true;

                IsRegistered = false;
                await LoadEventDetailsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отмены регистрации: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
