using EventFinderClient.Models.DTO;
using EventFinderClient.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly AuthService _authService;
        private readonly EventAttendeeService _attendeeService;

        private EventDetailsDto _event;
        private VenueDto _venue;
        private OrganizerDto _organizer;
        private int _eventId;
        private bool _isRegistered;
        private string _currentUserEmail = string.Empty;
        private string _currentUserName = string.Empty;

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
                            await CheckIfUserIsRegisteredAsync();
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

        public bool IsRegistered
        {
            get => _isRegistered;
            set => SetProperty(ref _isRegistered, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand UnregisterCommand { get; }
        public ICommand GoBackCommand { get; }

        public EventDetailsViewModel(IApiService apiService)
        {
            _eventService = new EventService(apiService);
            _venueService = new VenueService(apiService);
            _organizerService = new OrganizerService(apiService);
            _authService = new AuthService(apiService);
            _attendeeService = new EventAttendeeService(apiService);

            RegisterCommand = new Command(async () => await RegisterForEventAsync());
            UnregisterCommand = new Command(async () => await UnregisterFromEventAsync());
            GoBackCommand = new Command(async () => await GoBackAsync());

            Task.Run(async () => await LoadCurrentUserInfoAsync());
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

        private async Task LoadCurrentUserInfoAsync()
        {
            try
            {
                _currentUserEmail = await SecureStorage.Default.GetAsync("user_email");

                if (string.IsNullOrEmpty(_currentUserEmail))
                {
                    _currentUserEmail = Preferences.Default.Get("Email", string.Empty);
                }

                if (string.IsNullOrEmpty(_currentUserEmail))
                {
                    try
                    {
                        var profile = await _authService.GetProfileAsync();
                        if (profile != null)
                        {
                            _currentUserEmail = profile.Email;
                            _currentUserName = profile.Username;

                            await SecureStorage.Default.SetAsync("user_email", profile.Email);
                            await SecureStorage.Default.SetAsync("user_name", profile.Username);

                            Preferences.Default.Set("Email", profile.Email);
                            Preferences.Default.Set("Username", profile.Username);
                        }
                    }
                    catch 
                    {
                        
                    }
                }
                else
                {
                    _currentUserName = await SecureStorage.Default.GetAsync("user_name");
                    if (string.IsNullOrEmpty(_currentUserName))
                    {
                        _currentUserName = Preferences.Default.Get("Username", string.Empty);
                    }
                }
            }
            catch 
            {
               
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

        private async Task CheckIfUserIsRegisteredAsync()
        {
            try
            {
                if (Event == null)
                {
                    IsRegistered = false;
                    return;
                }

                var userEmail = await SecureStorage.Default.GetAsync("user_email");

                if (string.IsNullOrEmpty(userEmail))
                {
                    userEmail = Preferences.Default.Get("Email", string.Empty);
                }

                if (string.IsNullOrEmpty(userEmail))
                {
                    try
                    {
                        var profile = await _authService.GetProfileAsync();
                        if (profile != null)
                        {
                            userEmail = profile.Email;
                            await SecureStorage.Default.SetAsync("user_email", profile.Email);
                            Preferences.Default.Set("Email", profile.Email);
                        }
                    }
                    catch
                    {
                        
                    }
                }

                if (string.IsNullOrEmpty(userEmail))
                {
                    IsRegistered = false;
                    return;
                }
                        
                _currentUserEmail = userEmail;

                var attendees = await _attendeeService.IsUserRegisteredForEventAsync(Event.Id, _currentUserEmail);

                IsRegistered = attendees;
            }
            catch   
            {
                IsRegistered = false;
            }
        }

        private async Task RegisterForEventAsync()
        {
            try
            {
                if (Event == null)
                {
                    await ShowMessage("Ошибка", "Мероприятие не найдено");
                    return;
                }

                if (string.IsNullOrEmpty(_currentUserEmail))
                {
                    await ShowMessage("Ошибка", "Необходимо войти в систему");
                    return;
                }

                if (IsRegistered)
                {
                    await ShowMessage("Информация", "Вы уже зарегистрированы на это мероприятие");
                    return;
                }

                if (Event.CurrentAttendees >= Event.MaxAttendees)
                {
                    await ShowMessage("Ошибка", "На мероприятии нет свободных мест");
                    return;
                }

                IsBusy = true;

                var registerDto = new RegisterForEventDto
                {
                    UserName = _currentUserName,
                    Email = _currentUserEmail
                };

                var success = await _attendeeService.RegisterForEventAsync(Event.Id, registerDto);

                if (success)
                {
                    IsRegistered = true;

                    if (Event != null)
                    {
                        Event.CurrentAttendees++;
                        OnPropertyChanged(nameof(Event));
                    }

                    await RefreshEventAttendeesAsync();

                    await ShowMessage("Успех", "Вы успешно зарегистрировались на мероприятие");
                }
                else
                {
                    await ShowMessage("Ошибка", $"Не удалось зарегистрироваться на мероприятие");
                }
            }
            catch
            {
                await ShowMessage("Ошибка", "Произошла ошибка при регистрации");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UnregisterFromEventAsync()
        {
            try
            {
                if (Event == null || string.IsNullOrEmpty(_currentUserEmail))
                {
                    await ShowMessage("Ошибка", "Не удалось определить пользователя");
                    return;
                }
                    
                var registrationId = await _attendeeService.GetRegistrationIdAsync(Event.Id, _currentUserEmail);

                if (!registrationId.HasValue)
                {
                    await ShowMessage("Информация", "Вы не зарегистрированы на это мероприятие");
                    return;
                }

                IsBusy = true;

                var success = await _attendeeService.CancelRegistrationAsync(registrationId.Value);

                if (success)
                {
                    IsRegistered = false;

                    if (Event != null && Event.CurrentAttendees > 0)
                    {
                        Event.CurrentAttendees--;
                        OnPropertyChanged(nameof(Event));
                    }

                    await RefreshEventAttendeesAsync();

                    await ShowMessage("Успех", "Регистрация на мероприятие отменена");
                }
                else
                {
                    await ShowMessage("Ошибка", "Не удалось отменить регистрацию");
                }
            }
            catch (Exception ex)
            {
                await ShowMessage("Ошибка", "Произошла ошибка при отмене регистрации");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshEventAttendeesAsync()
        {
            try
            {
                if (Event == null) return;

                var attendees = await _attendeeService.GetAttendeesForEventAsync(Event.Id);

                if (Event.Attendees == null)
                {
                    Event.Attendees = new List<EventAttendeeDto>();
                }
                else
                {
                    Event.Attendees.Clear();
                }

                if (attendees != null)
                {
                    foreach (var attendee in attendees)
                    {
                        Event.Attendees.Add(attendee);
                    }
                }

                Event.CurrentAttendees = Event.Attendees.Count;

                await CheckIfUserIsRegisteredAsync();

                OnPropertyChanged(nameof(Event));
            }
            catch
            {
               
            }
        }

        private async Task ShowMessage(string title, string message)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            });
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
