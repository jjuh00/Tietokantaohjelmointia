using Lentolippujärjestelmä.Models;
using System;
using Microsoft.Maui.Controls;

namespace Lentolippujärjestelmä.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginBtnClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;

            // Toistaiseksi, tarkistetaan vain perustiedot
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Virhe", "Syötä kaikki tiedot!", "OK");
                return;
            }

            // TODO: Tähän toteutetaan käyttäjäntunnistuksen logiikka
        }

        private async void OnRegisterLabelTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }
    }
}