using Lentolippujärjestelmä.Services;
using Lentolippujärjestelmä.Models;
using System.Text.RegularExpressions;

namespace Lentolippujärjestelmä.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly PasswordHashingService pwhash;
        private readonly DatabaseService db;

        public LoginPage(PasswordHashingService passwordHashingService, DatabaseService databaseService)
        {
            InitializeComponent();
            pwhash = passwordHashingService;
            db = databaseService;
            CheckDatabaseConnection();
        }

        private async void CheckDatabaseConnection()
        {
            bool isConnected = await db.TestConnectionAsync();
            if (!isConnected)
            {
                await DisplayAlert("Tietokantavirhe", "Yhteyttä tietokantaan ei voitu muodostaa. Tarkista tietokanta-asetukset", "OK");
            }
        }

        private async void OnLoginBtnClicked(object sender, EventArgs e)
        {
            try
            {
                string email = EmailEntry.Text.Trim();
                string password = PasswordEntry.Text;

                // Tarkistetaan sähköposti ja salasana
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    await DisplayAlert("Virhe", "Syötä kaikki tiedot!", "OK");
                    return;
                }

                if (!IsValidEmail(email))
                {
                    await DisplayAlert("Virhe", "Anna kelvollinen sähköposti", "OK");
                    return;
                }

                // Haetaan käyttäjätietokannasta
                User user = await db.GetUserByEmailAsync(email);
                if (user == null || !pwhash.VerifyPassword(user.PasswordHash, password))
                {
                    await DisplayAlert("Virhe", "Väärä sähköposti tai salasana", "OK");
                    return;
                }

                // Navigointi roolin perusteella
                await Shell.Current.GoToAsync(user.Role == 0 ? nameof(AdminPage) : nameof(UserPage), new Dictionary<string, object> { { "User", user } });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Virhe kirjautumisessa: {ex.Message}", "OK");
            }
        }

        private async void OnRegisterLabelTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegistrationPage));
        }

        private static bool IsValidEmail(string email)
        {
            // Tarkistetaan sähköposti regexin avulla
            // vähintään 1 merkki ennen @, väh. 1 merkki sen jälkeen ja joku pääte
            return Regex.IsMatch(email, @"^.+@.+\..+$");
        }
    }
}