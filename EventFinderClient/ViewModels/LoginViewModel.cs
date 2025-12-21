using EventFinderClient.Models.DTO;
using EventFinderClient.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace EventFinderClient.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly AuthService _authService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(IApiService apiService)
        {
            _apiService = apiService;
            _authService = new AuthService(apiService);

            LoginCommand = new Command(async () => await LoginAsync());
            RegisterCommand = new Command(async () => await RegisterAsync());
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Пожалуйста, заполните все поля";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var loginRequest = new LoginRequestDto
                {
                    Email = Email,
                    Password = Password
                };

                var authResponse = await _authService.LoginAsync(loginRequest);

                if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                {
                    await SecureStorage.SetAsync("auth_token", authResponse.Token);

                    _apiService.SetAuthorizationHeader(authResponse.Token);

                    await Shell.Current.GoToAsync("//EventsPage");
                }
                else
                {
                    ErrorMessage = "Ошибка авторизации";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RegisterAsync()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }
    }
}