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
                _apiService.SetAuthorizationHeader(token);
                await Shell.Current.GoToAsync("//EventsPage");
            }
            else
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}
