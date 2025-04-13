using Lentolippujärjestelmä.Services;
using Lentolippujärjestelmä.Models;
using System.Text.RegularExpressions;

namespace Lentolippujärjestelmä.Views
{
    public partial class RegistrationPage : ContentPage
    {
        private readonly PasswordHashingService pwhash;
        private readonly DatabaseService db;

        public RegistrationPage(PasswordHashingService passwordHashingService, DatabaseService databaseService)
        {
            InitializeComponent();
            pwhash = passwordHashingService;
            db = databaseService;
        }

        private async void OnRegisterBtnClicked(object sender, EventArgs e)
        {
            try
            {
                string name = NameEntry.Text;
                string email = EmailEntry.Text;
                string password = PasswordEntry.Text;
                string confirmPassword = ConfirmPasswordEntry.Text;
                int role = AdminUserRadio.IsChecked ? 0 : 1;

                // Tarkistetaan käyttäjän antamat tiedot
                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(confirmPassword))
                {
                    await DisplayAlert("Virhe", "Täytä kaikki kentät!", "OK");
                    return;
                }

                if (!IsValidEmail(email))
                {
                    await DisplayAlert("Virhe", "Anna kelvollinen sähköposti", "OK");
                    return;
                } 

                if (password != confirmPassword)
                {
                    await DisplayAlert("Virhe", "Salasanat eivät täsmää", "OK");
                    return;
                }

                // Tarkistetaan, onko sähköposti jo käytössä
                if (await db.EmailExistsAsync(email))
                {
                    await DisplayAlert("Virhe", "Sähköposti on jo käytössä", "OK");
                    return;
                }

                // Luodaan ja tallennetaan käyttäjä
                var user = new User
                {
                    Name = name,
                    Email = email,
                    PasswordHash = pwhash.HashPassword(password),
                    Role = role
                };

                await db.InsertUserAsync(user);

                // Navigoidaan roolin perusteella
                await Shell.Current.GoToAsync(user.Role == 0 ? nameof(AdminPage) : nameof(UserPage), new Dictionary<string, object> { { "User", user } });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Virhe rekisteröitymisessä: {ex.Message}", "OK");
            }
        }

        private async void OnLoginLabelTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private static bool IsValidEmail(string email)
        {
            // Tarkistetaan sähköposti regexin avulla
            // vähintään 1 merkki ennen @, väh. 1 merkki sen jälkeen ja joku pääte
            return Regex.IsMatch(email, @"^.+@.+\..+$");
        }
    }
}