using Lentolippujärjestelmä.Services;
using Lentolippujärjestelmä.Views;
using Microsoft.Extensions.Logging;

namespace Lentolippujärjestelmä
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
            // Rekisteröidään palvelut
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<PasswordHashingService>();

            // Rekisteröidään sivut
            builder.Services.AddTransient<Views.LoginPage>();
            builder.Services.AddTransient<Views.RegistrationPage>();
            builder.Services.AddTransient<Views.UserPage>();
            builder.Services.AddTransient<Views.AdminPage>();
            builder.Services.AddTransient<Views.FlightListPage>();
            builder.Services.AddTransient<Views.ReservationsPage>();
            builder.Services.AddTransient<AddFlightPage>(provider =>
            {
                var dbService = provider.GetRequiredService<DatabaseService>();
                var editingFlight = FlightSession.EditingFlight;
                return new AddFlightPage(dbService, editingFlight);
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }

}