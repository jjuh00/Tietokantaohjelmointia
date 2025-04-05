using Lentolippujärjestelmä.Models;
using System;
using Microsoft.Maui.Controls;

namespace Lentolippujärjestelmä.Views
{
    public partial class UserPage : ContentPage
    {
        private readonly User _currentUser;
        public UserPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            WelcomeLabel.Text = $"Tervetuloa, {_currentUser.Name}";
        }

        private async void OnSearchFlightsClicked(object sender, EventArgs e)
        {
            string from = DepartureEntry.Text;
            string to = DestinationEntry.Text;
            DateTime departureDate = FlightDatePicker.Date;

            // Tietojen tarkistus
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                await DisplayAlert("Virhe", "Täytä kaikki tiedot!", "OK");
                return;
            }

            // TODO: Tähän toteutetaan oikea lentojen haku logiikka
        }

        private async void OnViewReservationsClicked(object sender, EventArgs e) 
        {
            // Normaalisti haettaisiin käyttäjän varaukset
            // Normaalisti navigoitaisiin varaussivulle
        }

        private void OnLogOutClicked(object sender, EventArgs e)
        {
            Application.Current?.Quit();
        }
    }
}