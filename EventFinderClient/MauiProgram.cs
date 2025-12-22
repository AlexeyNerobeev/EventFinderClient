using EventFinderClient.Services;
using EventFinderClient.ViewModels;
using EventFinderClient.Views;
using Microsoft.Extensions.Logging;

namespace EventFinderClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Сервисы
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<EventService>();
            builder.Services.AddSingleton<VenueService>();
            builder.Services.AddSingleton<OrganizerService>();
            builder.Services.AddSingleton<EventAttendeeService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<EventsViewModel>();
            builder.Services.AddTransient<EventDetailsViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();

            // Страницы
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<EventsPage>();
            builder.Services.AddTransient<EventDetailsPage>();
            builder.Services.AddTransient<ProfilePage>();

            // AppShell
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}