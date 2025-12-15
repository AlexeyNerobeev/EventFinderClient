using EventFinderClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EventFinderClient.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        private string _username = "Иванов Иван";
        private string _email = "ivanov@mail.ru";
        private string _role = "Пользователь";

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

        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        public ICommand LogoutCommand { get; }

        public ProfileViewModel()
        {
            _authService = new AuthService(null);
            LogoutCommand = new Command(async () => await LogoutAsync());
        }

        private async Task LogoutAsync()
        {
            try
            {
                SecureStorage.Remove("auth_token");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выхода: {ex.Message}");
            }
        }
    }
}
