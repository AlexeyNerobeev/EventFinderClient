using EventFinderClient.Services;

namespace EventFinderClient
{
    public partial class App : Application
    {
        private readonly IApiService _apiService;

        public App(AppShell shell, IApiService apiService)
        {
            InitializeComponent();
            MainPage = shell;
            _apiService = apiService;
        }

        protected override async void OnStart()
        {
            base.OnStart();

            var token = await SecureStorage.GetAsync("auth_token");

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    _apiService.SetAuthorizationHeader(token);

                    var authService = new AuthService(_apiService);
                    var profile = await authService.GetProfileAsync();

                    if (profile != null)
                    {
                        var storedEmail = await SecureStorage.GetAsync("user_email");
                        if (string.IsNullOrEmpty(storedEmail))
                        {
                            await SecureStorage.SetAsync("user_email", profile.Email);
                            await SecureStorage.SetAsync("user_name", profile.Username);
                        }

                        await Shell.Current.GoToAsync("//EventsPage");
                    }
                    else
                    {
                        await ClearAllUserData();
                        await Shell.Current.GoToAsync("//LoginPage");
                    }
                }
                catch
                {
                    await ClearAllUserData();
                    await Shell.Current.GoToAsync("//LoginPage");
                }
            }
            else
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        private async Task ClearAllUserData()
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
    }
}
