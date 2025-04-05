using Lentolippujärjestelmä.Models;
using System;
using Microsoft.Maui.Controls;

namespace Lentolippujärjestelmä.Views
{
    public partial class AdminPage : ContentPage
    {
        private readonly User _currentUser;
        public AdminPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            WelcomeLabel.Text = $"Tervetuloa, {_currentUser.Name}!";
        }

        private async void OnViewAllFlightsClicked(object sender, EventArgs e)
        {
            // Normaalisti haettaisiin ja näytettäisiin kaikki lennot
            // Normaalisti navigoitaisiin lennot-sivulle
        }

        private async void OnAddNewFlightClicked(object sender, EventArgs e)
        {
            // Normaalisti navigoitaisiin lomake-sivulle uuden lennon lisäämistä varten
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Application.Current?.Quit();
        }
    }
}