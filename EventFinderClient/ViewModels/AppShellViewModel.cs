using EventFinderClient.Services;
using EventFinderClient.Views;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventFinderClient.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        public Command LogoutCommand { get; }
        public Command NavigateToProfileCommand { get; }

        public AppShellViewModel()
        {
            LogoutCommand = new Command(async () => await ExecuteLogout());
            NavigateToProfileCommand = new Command(async () => await ExecuteNavigateToProfile());

            CheckLoginStatus();
        }

        private void CheckLoginStatus()
        {
            IsLoggedIn = !string.IsNullOrEmpty(Preferences.Get("AuthToken", ""));
        }

        private async Task ExecuteLogout()
        {
            try
            {
                SecureStorage.Remove("auth_token");
                SecureStorage.Remove("user_email");
                SecureStorage.Remove("user_name");
                SecureStorage.Remove("UserId");

                Preferences.Remove("AuthToken");
                Preferences.Remove("UserId");
                Preferences.Remove("Username");
                Preferences.Remove("Email");
                Preferences.Remove("Role");

                if (Application.Current?.Handler?.MauiContext?.Services != null)
                {
                    var apiService = Application.Current.Handler.MauiContext.Services.GetService<IApiService>();
                    apiService?.ClearAuthorizationHeader();
                }

                IsLoggedIn = false;

                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось выйти: {ex.Message}", "OK");
            }
        }

        private async Task ExecuteNavigateToProfile()
        {
            if (IsLoggedIn)
            {
                await Shell.Current.GoToAsync(nameof(ProfilePage));
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
        }
    }
}
