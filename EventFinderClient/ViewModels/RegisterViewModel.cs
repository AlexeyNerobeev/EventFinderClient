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
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly AuthService _authService;

        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _errorMessage = string.Empty;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        public RegisterViewModel(IApiService apiService)
        {
            _apiService = apiService;
            _authService = new AuthService(apiService);

            RegisterCommand = new Command(async () => await RegisterAsync());
            LoginCommand = new Command(async () => await LoginAsync());
        }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ErrorMessage = "Пожалуйста, заполните все поля";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Пароли не совпадают";
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Пароль должен содержать минимум 6 символов";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var registerRequest = new RegisterRequestDto
                {
                    Username = Username,
                    Email = Email,
                    Password = Password
                };

                var authResponse = await _authService.RegisterAsync(registerRequest);

                if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                {
                    await ClearOldUserData();

                    await SecureStorage.SetAsync("auth_token", authResponse.Token);
                    _apiService.SetAuthorizationHeader(authResponse.Token);

                    await SecureStorage.SetAsync("user_email", authResponse.Email);
                    await SecureStorage.SetAsync("user_name", authResponse.Username);
                    await SecureStorage.SetAsync("user_id", authResponse.UserId.ToString());

                    await Shell.Current.GoToAsync("//EventsPage");
                }
                else
                {
                    ErrorMessage = "Ошибка регистрации";
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

        private async Task ClearOldUserData()
        {
            try
            {
                SecureStorage.Remove("auth_token");
                SecureStorage.Remove("user_email");
                SecureStorage.Remove("user_name");
                SecureStorage.Remove("user_id");

                Preferences.Remove("AuthToken");
                Preferences.Remove("UserId");
                Preferences.Remove("Username");
                Preferences.Remove("Email");
                Preferences.Remove("Role");

                _apiService.ClearAuthorizationHeader();
            }
            catch
            {
                
            }
        }

        private async Task LoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
