using Lentolippujärjestelmä.Models;
using System;
using Microsoft.Maui.Controls;

namespace Lentolippujärjestelmä.Views
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterBtnClicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text;
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;
            string confirmPassword = ConfirmPasswordEntry.Text;
            bool isAdmin = AdminUserRadio.IsChecked;

            // Tarkistetaan käyttäjän tiedot
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Virhe", "Täytä kaikki kentät!", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Virhe", "Salasanat eivät täsmää", "OK");
                return;
            }

            // TODO: lisätään oikea käyttäjän rekisteröinti logiikka

            // Luodaan user-olio annettuilla tiedoilla
            User user = new User
            {
                Name = name,
                Email = email,
                IsAdmin = isAdmin
            };

            // Navigoidaan oikealle sivulle riippuen käyttäjän roolista
            if (user.IsAdmin)
            {
                await Navigation.PushAsync(new AdminPage(user));
            }
            else
            {
                await Navigation.PushAsync(new UserPage(user));
            }
        }

        private async void OnLoginLabelTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}