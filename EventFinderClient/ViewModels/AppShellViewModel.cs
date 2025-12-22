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
            LogoutCommand = new Command(ExecuteLogout);
            NavigateToProfileCommand = new Command(ExecuteNavigateToProfile);

            CheckLoginStatus();
        }

        private void CheckLoginStatus()
        {
            IsLoggedIn = !string.IsNullOrEmpty(Preferences.Get("AuthToken", ""));
        }

        private async void ExecuteLogout()
        {
            Preferences.Remove("AuthToken");
            Preferences.Remove("UserId");
            Preferences.Remove("Username");
            Preferences.Remove("Email");
            Preferences.Remove("Role");

            IsLoggedIn = false;

            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        private async void ExecuteNavigateToProfile()
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
