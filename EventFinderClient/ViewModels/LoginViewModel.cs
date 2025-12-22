using EventFinderClient.Models.DTO;
using EventFinderClient.Services;
using System.ComponentModel;
using System.Diagnostics;
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

                Debug.WriteLine($"[LoginViewModel] Начало входа для {Email}");

                var loginRequest = new LoginRequestDto
                {
                    Email = Email,
                    Password = Password
                };

                var authResponse = await _authService.LoginAsync(loginRequest);

                if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                {
                    await ClearAllUserData();

                    await SecureStorage.Default.SetAsync("auth_token", authResponse.Token);

                    await SecureStorage.Default.SetAsync("user_email", authResponse.Email);
                    await SecureStorage.Default.SetAsync("user_name", authResponse.Username);
                    await SecureStorage.Default.SetAsync("user_id", authResponse.UserId.ToString());
                    await SecureStorage.Default.SetAsync("user_role", authResponse.Role);

                    Preferences.Default.Set("AuthToken", authResponse.Token);
                    Preferences.Default.Set("UserId", authResponse.UserId.ToString());
                    Preferences.Default.Set("Username", authResponse.Username);
                    Preferences.Default.Set("Email", authResponse.Email);
                    Preferences.Default.Set("Role", authResponse.Role);

                    Debug.WriteLine($"[LoginViewModel] Данные нового пользователя сохранены: {authResponse.Email}");

                    _apiService.SetAuthorizationHeader(authResponse.Token);

                    Email = string.Empty;
                    Password = string.Empty;

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

        private async Task ClearAllUserData()
        {
            try
            {
                SecureStorage.Default.Remove("auth_token");
                SecureStorage.Default.Remove("user_email");
                SecureStorage.Default.Remove("user_name");
                SecureStorage.Default.Remove("user_id");
                SecureStorage.Default.Remove("user_role");

                Preferences.Default.Remove("AuthToken");
                Preferences.Default.Remove("UserId");
                Preferences.Default.Remove("Username");
                Preferences.Default.Remove("Email");
                Preferences.Default.Remove("Role");

                _apiService.ClearAuthorizationHeader();

            }
            catch 
            {
                
            }
        }

        private async Task RegisterAsync()
        {
            await Shell.Current.GoToAsync("RegisterPage");
        }
    }
}