using Lentolippujärjestelmä.Models;
using Lentolippujärjestelmä.Services;

namespace Lentolippujärjestelmä.Views
{
    [QueryProperty(nameof(CurrentUser), "User")]
    public partial class ReservationsPage : ContentPage
    {
        private User _currentUser;
        private readonly DatabaseService db;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public ReservationsPage(DatabaseService databaseService)
        {
            InitializeComponent();
            db = databaseService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_currentUser == null)
            {
                await DisplayAlert("Virhe", "Käyttäjän tietoja ei saatu", "OK");
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync(nameof(LoginPage));
                return;
            }
            await LoadReservations();
        }

        // Ladataan ja näytetään käyttäjän varaukset
        private async Task LoadReservations()
        {
            try
            {
                var reservations = await db.GetReservationsAsync(_currentUser.Id);
                ReservationsListView.ItemsSource = reservations;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Virhe", $"Varausten lataaminen epäonnistui: {ex.Message}", "OK");
            }
        }
    }
}