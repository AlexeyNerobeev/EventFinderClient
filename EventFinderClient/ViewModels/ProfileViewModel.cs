using EventFinderClient.Services;
using EventFinderClient.Views;
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

        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _role = string.Empty;
        private string _createdAt = string.Empty;

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

        public string CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }

        public ICommand LoadProfileCommand { get; }
        public ICommand LogoutCommand { get; }

        public ProfileViewModel(IApiService apiService)
        {
            _authService = new AuthService(apiService);

            LoadProfileCommand = new Command(async () => await LoadProfileAsync());
            LogoutCommand = new Command(async () => await LogoutAsync());

            Task.Run(async () => await LoadProfileAsync());
        }

        private async Task LoadProfileAsync()
        {
            try
            {
                IsBusy = true;

                var profile = await _authService.GetProfileAsync();

                if (profile != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Username = profile.Username;
                        Email = profile.Email;
                        Role = GetRoleDisplayName(profile.Role);
                        CreatedAt = profile.CreatedAt.ToString("dd.MM.yyyy");
                    });
                }
            }
            catch
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Username = "Не загружено";
                    Email = "Не загружено";
                    Role = "Не загружено";
                    CreatedAt = "Не загружено";
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        private string GetRoleDisplayName(string role)
        {
            return role switch
            {
                "Admin" => "Администратор",
                "Organizer" => "Организатор",
                "User" => "Пользователь",
                _ => role
            };
        }

        private async Task LogoutAsync()
        {
            try
            {
                SecureStorage.Remove("auth_token");

                if (Application.Current.Handler?.MauiContext?.Services != null)
                {
                    var apiService = Application.Current.Handler.MauiContext.Services.GetService<IApiService>();
                    apiService?.ClearAuthorizationHeader();
                }

                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch
            {
                
            }
        }

        public void OnAppearing()
        {
            Task.Run(async () => await LoadProfileAsync());
        }
    }
}
